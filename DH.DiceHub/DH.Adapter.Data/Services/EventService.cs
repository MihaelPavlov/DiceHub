using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

internal class EventService : IEventService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;

    public EventService(IDbContextFactory<TenantDbContext> _contextFactory)
    {
        this._contextFactory = _contextFactory;
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

    public async Task<List<GetEventListQueryModel>> GetListBySearchExpressionAsync(string searchExpression, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
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
                }).ToListAsync(cancellationToken);
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
