using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

internal class EventService : IEventService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;

    public EventService(IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
    }

    public async Task<int> CreateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == eventModel.GameId)
                ?? throw new NotFoundException(nameof(Game), eventModel.GameId);

            await context.Events.AddAsync(eventModel, cancellationToken);

            if (eventModel.IsCustomImage &&
                fileName != null &&
                contentType != null &&
                imageStream != null)
            {
                await context.EventImages
                    .AddAsync(new EventImage
                    {
                        Event = eventModel,
                        FileName = fileName,
                        ContentType = contentType,
                        Data = imageStream.ToArray(),
                    }, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            return eventModel.Id;
        }
    }

    public async Task<GetEventByIdQueryModel?> GetById(int eventId, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await (
                from e in context.Events
                join g in context.Games on e.GameId equals g.Id
                join ei in context.EventImages on e.Id equals ei.EventId into eventImages
                from ei in eventImages.DefaultIfEmpty()
                where e.Id == eventId
                select new GetEventByIdQueryModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageId = e.IsCustomImage ? ei.Id : g.Image.Id,
                    GameId = g.Id,
                    GameName = g.Name,
                    GameDescription = g.Description,
                    GameAveragePlaytime = g.AveragePlaytime,
                    GameMinAge = g.MinAge,
                    GameMaxPlayers = g.MaxPlayers,
                    GameMinPlayers = g.MinPlayers,
                }).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public async Task<List<GetEventListQueryModel>> GetListForUsers(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var today = DateTime.UtcNow;
            return await (
                from e in context.Events
                join g in context.Games on e.GameId equals g.Id
                join ei in context.EventImages on e.Id equals ei.EventId into eventImages
                from ei in eventImages.DefaultIfEmpty()
                where today.Date <= e.StartDate.Date
                select new GetEventListQueryModel
                {
                    Id = e.Id,
                    GameId = g.Id,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageId = e.IsCustomImage ? ei.Id : g.Image.Id,
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
                from e in context.Events
                join g in context.Games on e.GameId equals g.Id
                join ei in context.EventImages on e.Id equals ei.EventId into eventImages
                from ei in eventImages.DefaultIfEmpty()
                where e.Name.ToLower().Contains(searchExpression.ToLower())
                select new GetEventListQueryModel
                {
                    Id = e.Id,
                    GameId = g.Id,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.Participants.Count,
                    ImageId = e.IsCustomImage ? ei.Id : g.Image.Id,
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
            return await (
                from ep in context.EventParticipants
                join g in context.Games on ep.Event.GameId equals g.Id
                join ei in context.EventImages on ep.Event.Id equals ei.EventId into eventImages
                from ei in eventImages.DefaultIfEmpty()
                select new GetEventListQueryModel
                {
                    Id = ep.Event.Id,
                    GameId = g.Id,
                    Name = ep.Event.Name,
                    StartDate = ep.Event.StartDate,
                    IsCustomImage = ep.Event.IsCustomImage,
                    MaxPeople = ep.Event.MaxPeople,
                    PeopleJoined = ep.Event.Participants.Count,
                    ImageId = ep.Event.IsCustomImage ? ei.Id : g.Image.Id,
                })
                .OrderBy(x => x.StartDate)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task UpdateEvent(Event eventModel, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var eventDb = await context.Events.AsTracking().FirstOrDefaultAsync(x => x.Id == eventModel.Id)
                ?? throw new NotFoundException(nameof(Event), eventModel.Id);

            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == eventModel.GameId)
                ?? throw new NotFoundException(nameof(Game), eventModel.GameId);

            eventDb.Name = eventModel.Name;
            eventDb.Description = eventModel.Description;
            eventDb.StartDate = eventModel.StartDate;
            eventDb.MaxPeople = eventModel.MaxPeople;
            eventDb.GameId = eventModel.GameId;

            context.Events.Update(eventDb);

            if (eventDb.IsCustomImage != eventModel.IsCustomImage)
            {
                if (!eventDb.IsCustomImage &&
                    eventModel.IsCustomImage &&
                    fileName != null &&
                    contentType != null &&
                    imageStream != null)
                {
                    await context.EventImages
                    .AddAsync(new EventImage
                    {
                        Event = eventDb,
                        FileName = fileName,
                        ContentType = contentType,
                        Data = imageStream.ToArray(),
                    }, cancellationToken);
                }
                else if (eventDb.IsCustomImage && !eventModel.IsCustomImage)
                {
                    await context.EventImages.Where(x => x.EventId == eventDb.Id).ExecuteDeleteAsync(cancellationToken);
                }
            }

            eventDb.IsCustomImage = eventModel.IsCustomImage;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
