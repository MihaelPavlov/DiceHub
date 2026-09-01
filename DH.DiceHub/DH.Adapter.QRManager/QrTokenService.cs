using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using System.Security.Cryptography;

namespace DH.Adapter.QRManager;

/// <inheritdoc />
public class QrTokenService : IQrTokenService
{
    // Crockford base32 (no I/L/O/U); every char is in [0-9A-Z] so the QR uses
    // the compact alphanumeric mode.
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
    private const int RandomLength = 11;
    private static readonly TimeSpan PerUserLifetime = TimeSpan.FromMinutes(15);

    readonly IRepository<QrToken> repository;
    readonly IUserContext userContext;

    public QrTokenService(IRepository<QrToken> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    private static bool IsPerUser(QrCodeType type) => (int)type >= (int)QrCodeType.Reward;

    /// <inheritdoc />
    public async Task<string> IssueAsync(QrCodeType type, int entityId, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(QrCodeType), type) || type == QrCodeType.Unknown || entityId <= 0)
            throw new BadRequestException("Invalid QR code request.");

        var perUser = IsPerUser(type);
        var userId = perUser ? this.userContext.UserId : null;

        if (perUser && string.IsNullOrEmpty(userId))
            throw new BadRequestException("A signed-in user is required for this QR code.");

        var now = DateTime.UtcNow;

        // Reuse an existing usable token so re-opening the dialog doesn't pile up rows.
        var existing = await this.repository.GetByAsync(
            x => x.Type == type
                 && x.EntityId == entityId
                 && x.UserId == userId
                 && x.ConsumedDate == null
                 && (x.ExpiresDate == null || x.ExpiresDate > now),
            cancellationToken);

        if (existing != null)
            return existing.Token;

        for (var attempt = 0; attempt < 4; attempt++)
        {
            var token = Generate(type);
            var clash = await this.repository.GetByAsync(x => x.Token == token, cancellationToken);
            if (clash != null)
                continue;

            await this.repository.AddAsync(new QrToken
            {
                Token = token,
                Type = type,
                EntityId = entityId,
                UserId = userId,
                CreatedDate = now,
                ExpiresDate = perUser ? now.Add(PerUserLifetime) : null,
            }, cancellationToken);

            return token;
        }

        throw new InvalidOperationException("Could not allocate a unique QR token.");
    }

    /// <inheritdoc />
    public async Task<QRReaderModel?> ResolveAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var row = await this.repository.GetByAsync(x => x.Token == token, cancellationToken);
        if (row == null)
            return null;

        var now = DateTime.UtcNow;
        if (row.ConsumedDate != null || (row.ExpiresDate != null && row.ExpiresDate <= now))
            return null;

        var model = new QRReaderModel
        {
            Id = row.EntityId,
            Type = row.Type,
            Name = row.Type.ToString(),
        };

        if (!string.IsNullOrEmpty(row.UserId))
            model.AdditionalData["userId"] = row.UserId;

        return model;
    }

    /// <inheritdoc />
    public async Task MarkConsumedAsync(string token, CancellationToken cancellationToken)
    {
        var row = await this.repository.GetByAsyncWithTracking(x => x.Token == token, cancellationToken);

        // Only per-user codes are single use; the static Game/Event ones stay reusable.
        if (row == null || string.IsNullOrEmpty(row.UserId) || row.ConsumedDate != null)
            return;

        row.ConsumedDate = DateTime.UtcNow;
        await this.repository.SaveChangesAsync(cancellationToken);
    }

    private static string Generate(QrCodeType type)
    {
        Span<char> chars = stackalloc char[1 + RandomLength];
        chars[0] = (char)('0' + (int)type); // 1 digit type hint for the scanner UI

        Span<byte> bytes = stackalloc byte[RandomLength];
        RandomNumberGenerator.Fill(bytes);
        for (var i = 0; i < RandomLength; i++)
            chars[1 + i] = Alphabet[bytes[i] % Alphabet.Length];

        return new string(chars);
    }
}
