﻿using DH.Domain.Adapters.Statistics.Services;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Domain.Adapters.Statistics.JobHandlers;

public class EventAttendanceDetectedJobHandler : IStatisticJob
{
    public Guid JobId => this.job.JobId;

    readonly EventAttendanceDetectedJob job;
    readonly IStatisticsService statisticsService;

    public EventAttendanceDetectedJobHandler(EventAttendanceDetectedJob job, IStatisticsService statisticsService)
    {
        this.job = job;
        this.statisticsService = statisticsService;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.statisticsService.EventAttendanceDetectedMessage(this.job);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
