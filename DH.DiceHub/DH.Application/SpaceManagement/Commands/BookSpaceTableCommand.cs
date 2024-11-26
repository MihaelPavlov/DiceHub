using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;
using System.Globalization;

namespace DH.Application.SpaceManagement.Commands;

public record BookSpaceTableCommand(int NumberOfGuests, DateTime ReservationDate, string Time) : IRequest;

internal class BookSpaceTableCommandHandler(IRepository<SpaceTableReservation> repository, IUserContext userContext) : IRequestHandler<BookSpaceTableCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IUserContext userContext = userContext;

    public async Task Handle(BookSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var isUserHaveActiveReservation = await this.repository.GetByAsync(x => x.IsActive && x.UserId == this.userContext.UserId, cancellationToken);

        if (isUserHaveActiveReservation != null)
            throw new BadRequestException("User already have an active reservation");

        await this.repository.AddAsync(new SpaceTableReservation
        {
            UserId = this.userContext.UserId,
            CreatedDate = DateTime.UtcNow,
            ReservationDate = CombineDateAndTime(request.ReservationDate, request.Time),
            IsReservationSuccessful = false,
            IsActive = true,
            NumberOfGuests = request.NumberOfGuests,
            IsConfirmed = false,
        }, cancellationToken);
    }

    public DateTime CombineDateAndTime(DateTime reservationDate, string time)
    {
        // Parse the time string into a TimeSpan
        if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out var parsedTime))
            throw new FormatException("Time format is invalid. Expected format is 'HH:mm'.");

        // Combine the date and time
        return reservationDate.Date.Add(parsedTime);
    }
}