using MediatR;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Enums;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;

namespace DH.Application.Statistics.Queries;

public record GetUserWhoPlayedGameChartDataQuery(int GameId, ChartGameActivityType Type, DateTime? RangeStart, DateTime? RangeEnd) : IRequest<OperationResult<GetUsersWhoPlayedGameData>>;

internal class GetUserWhoPlayedGameChartDataQueryHandler(
    IStatisticsService statisticsService,
    IUserManagementService userManagementService) : IRequestHandler<GetUserWhoPlayedGameChartDataQuery, OperationResult<GetUsersWhoPlayedGameData>>
{
    readonly IStatisticsService statisticsService = statisticsService;
    readonly IUserManagementService userManagementService = userManagementService;
    public async Task<OperationResult<GetUsersWhoPlayedGameData>> Handle(
        GetUserWhoPlayedGameChartDataQuery request, CancellationToken cancellationToken)
    {
        var operationResult = await statisticsService.GetGameActivityUsersData(
            request.GameId, request.Type, request.RangeStart, request.RangeEnd, cancellationToken);

        var userIds = operationResult.RelatedObject?.Users.Select(x => x.UserId).ToArray();
        var users = await this.userManagementService.GetUserListByIds(userIds ?? [], cancellationToken);

        foreach (var user in operationResult.RelatedObject?.Users ?? [])
        {
            var dbUser = users.FirstOrDefault(x => x.Id == user.UserId);

            if (dbUser is null) continue;

            user.UserDisplayName = dbUser.UserName;
        }

        return operationResult;
    }
}
