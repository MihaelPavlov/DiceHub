using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

internal class EventService : IEventService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;
    readonly IFileManagerClient fileManagerClient;

    public EventService(IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext, IFileManagerClient fileManagerClient)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
        this.fileManagerClient = fileManagerClient;
    }

    public async Task<int> CreateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == eventModel.GameId)
                ?? throw new NotFoundException(nameof(Game), eventModel.GameId);

            if (eventModel.IsCustomImage &&
                fileName != null &&
                contentType != null &&
                imageStream != null)
            {
                var imageUrl = await this.fileManagerClient.UploadFileAsync(
                    FileManagerFolders.Events.ToString(), fileName, imageStream.ToArray());
                eventModel.ImageUrl = imageUrl;
            }
            await context.Events.AddAsync(eventModel, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return eventModel.Id;
        }
    }

    public async Task<GetEventByIdQueryModel?> GetById(int eventId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from e in context.Events.AsNoTracking()
                where e.Id == eventId && !e.IsDeleted
                select new GetEventByIdQueryModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description_EN = e.Description_EN,
                    Description_BG = e.Description_BG,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageUrl = e.IsCustomImage ? e.ImageUrl : e.Game.ImageUrl,
                    GameId = e.Game.Id,
                    GameName = e.Game.Name,
                    GameDescription_EN = e.Game.Description_EN,
                    GameDescription_BG = e.Game.Description_BG,
                    GameAveragePlaytime = e.Game.AveragePlaytime,
                    GameMinAge = e.Game.MinAge,
                    GameMaxPlayers = e.Game.MaxPlayers,
                    GameMinPlayers = e.Game.MinPlayers,
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<List<GetEventListQueryModel>> GetListForUsers(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var today = DateTime.UtcNow;
            return await (
                from e in context.Events.AsNoTracking()
                where today.Date <= e.StartDate.Date && !e.IsDeleted
                select new GetEventListQueryModel
                {
                    Id = e.Id,
                    GameId = e.Game.Id,
                    Name = e.Name,
                    Description_EN = e.Description_EN,
                    Description_BG = e.Description_BG,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageUrl = e.IsCustomImage ? e.ImageUrl : e.Game.ImageUrl,
                })
                .OrderBy(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetEventListQueryModel>> GetListForStaff(string searchExpression, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var today = DateTime.UtcNow;

            return await (
                from e in context.Events.AsNoTracking()
                where e.Name.ToLower().Contains(searchExpression.ToLower()) && !e.IsDeleted
                select new GetEventListQueryModel
                {
                    Id = e.Id,
                    GameId = e.Game.Id,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageUrl = e.IsCustomImage ? e.ImageUrl : e.Game.ImageUrl,
                })
                .OrderBy(x => x.StartDate < today ? 1 : 0)   // Past events (1) go to the bottom
                .ThenBy(x => x.StartDate >= today ? x.StartDate : DateTime.MaxValue) // Ascending for future/today
                .ThenByDescending(x => x.StartDate < today ? x.StartDate : DateTime.MinValue) // Descending for past
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<List<GetEventListQueryModel>> GetUserEvents(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var today = DateTime.UtcNow;

            return await (
                from ep in context.EventParticipants.AsNoTracking()
                where ep.UserId == this.userContext.UserId && today.Date <= ep.Event.StartDate.Date && !ep.Event.IsDeleted
                select new GetEventListQueryModel
                {
                    Id = ep.Event.Id,
                    GameId = ep.Event.Game.Id,
                    Name = ep.Event.Name,
                    Description_EN = ep.Event.Description_EN,
                    Description_BG = ep.Event.Description_BG,
                    StartDate = ep.Event.StartDate,
                    IsCustomImage = ep.Event.IsCustomImage,
                    MaxPeople = ep.Event.MaxPeople,
                    PeopleJoined = ep.Event.Participants.Count,
                    ImageUrl = ep.Event.IsCustomImage ? ep.Event.ImageUrl : ep.Event.Game.ImageUrl,
                })
                .OrderBy(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<UpdateEventResponseModel> UpdateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var eventDb = await context.Events.AsTracking().FirstOrDefaultAsync(x => x.Id == eventModel.Id)
                ?? throw new NotFoundException(nameof(Event), eventModel.Id);

            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == eventModel.GameId)
                ?? throw new NotFoundException(nameof(Game), eventModel.GameId);

            var response = new UpdateEventResponseModel()
            {
                ShouldSendStarDateUpdatedNotification = false,
            };

            eventDb.Name = eventModel.Name;
            eventDb.Description_BG = eventModel.Description_BG;
            eventDb.Description_EN = eventModel.Description_EN;
            if (eventDb.StartDate != eventModel.StartDate)
            {
                response.ShouldSendStarDateUpdatedNotification = true;
            }
            eventDb.StartDate = eventModel.StartDate;
            eventDb.MaxPeople = eventModel.MaxPeople;
            eventDb.GameId = eventModel.GameId;

            if (eventModel.IsCustomImage)
            {
                if (fileName != null &&
                    contentType != null &&
                    imageStream != null)
                {
                    string? currenFileImageUrl = null;

                    if (!string.IsNullOrEmpty(fileName))
                        currenFileImageUrl = this.fileManagerClient.GetPublicUrl(
                            FileManagerFolders.Events.ToString(), fileName);

                    var imageUrl = await this.fileManagerClient.UploadFileAsync(
                        FileManagerFolders.Events.ToString(), fileName, imageStream.ToArray());
                    eventDb.ImageUrl = imageUrl;
                }
            }
            eventDb.IsCustomImage = eventModel.IsCustomImage;

            await context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}
