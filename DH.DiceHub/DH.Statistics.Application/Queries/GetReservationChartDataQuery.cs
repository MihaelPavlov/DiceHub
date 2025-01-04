using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Enums;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetReservationChartDataQuery(string FromDate, string? ToDate) : IRequest<OperationResult<GetReservationChartData>>;

internal class GetReservationChartDataQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetReservationChartDataQuery, OperationResult<GetReservationChartData>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<GetReservationChartData>> Handle(GetReservationChartDataQuery request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(request.FromDate, out var fromDateUtc))
            return new OperationResult<GetReservationChartData>().ReturnWithBadRequestException("From Date is Missing or Incorrect");

        if (!DateTime.TryParse(request.ToDate, out var toDateUtc))
            return new OperationResult<GetReservationChartData>().ReturnWithBadRequestException("To Date is Missing or Incorrect");

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var reservations = await context.ReservationOutcomeLogs
                .Where(x => x.OutcomeDate.Date >= fromDateUtc.Date && x.OutcomeDate.Date <= toDateUtc.Date)
                .ToListAsync(cancellationToken);

            var gameReservations = reservations.Where(r => r.Type == ReservationType.Game)
                .GroupBy(r => r.Outcome)
                .Select(g => new
                {
                    Outcome = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(g => g.Outcome, g => g.Count);

            var tableReservations = reservations.Where(r => r.Type == ReservationType.Table)
                .GroupBy(r => r.Outcome)
                .Select(g => new
                {
                    Outcome = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(g => g.Outcome, g => g.Count);

            var gameStats = new ReservationStats
            {
                Completed = gameReservations.ContainsKey(ReservationOutcome.Completed) ? gameReservations[ReservationOutcome.Completed] : 0,
                Cancelled = gameReservations.ContainsKey(ReservationOutcome.Cancelled) ? gameReservations[ReservationOutcome.Cancelled] : 0
            };

            var tableStats = new ReservationStats
            {
                Completed = tableReservations.ContainsKey(ReservationOutcome.Completed) ? tableReservations[ReservationOutcome.Completed] : 0,
                Cancelled = tableReservations.ContainsKey(ReservationOutcome.Cancelled) ? tableReservations[ReservationOutcome.Cancelled] : 0
            };

            var result = new GetReservationChartData
            {
                GameReservationStats = gameStats,
                TableReservationStats = tableStats
            };

            return new OperationResult<GetReservationChartData>(result);
        }
    }
}
