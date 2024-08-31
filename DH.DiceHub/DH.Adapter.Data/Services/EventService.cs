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
                select new GetEventByIdQueryModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    IsCustomImage = e.IsCustomImage,
                    MaxPeople = e.MaxPeople,
                    PeopleJoined = e.PeopleJoined,
                    ImageId = e.IsCustomImage ? ei.Id : g.Image.Id,
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
                    PeopleJoined = e.PeopleJoined,
                    ImageId = e.IsCustomImage ? ei.Id : g.Image.Id,
                }).ToListAsync(cancellationToken);
        }
    }
}
