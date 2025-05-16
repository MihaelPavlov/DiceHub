using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetEventAttendanceChartDataQuery(string FromDate, string ToDate) : IRequest<OperationResult<GetEventAttendanceChartData>>;

internal class GetEventAttendanceChartDataQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetEventAttendanceChartDataQuery, OperationResult<GetEventAttendanceChartData>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetEventAttendanceChartData>> Handle(GetEventAttendanceChartDataQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetEventAttendanceChartData(request.FromDate, request.ToDate, cancellationToken);
    }
}