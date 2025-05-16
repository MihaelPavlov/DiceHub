using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetReservationChartDataQuery(string FromDate, string ToDate) : IRequest<OperationResult<GetReservationChartData>>;

internal class GetReservationChartDataQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetReservationChartDataQuery, OperationResult<GetReservationChartData>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetReservationChartData>> Handle(GetReservationChartDataQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetReservationChartData(request.FromDate, request.ToDate, cancellationToken);
    }
}
