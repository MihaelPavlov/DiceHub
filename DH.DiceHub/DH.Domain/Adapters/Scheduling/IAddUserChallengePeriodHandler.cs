﻿namespace DH.Domain.Adapters.Scheduling;

public interface IAddUserChallengePeriodHandler
{   
    Task InitializeNewPeriods(CancellationToken cancellationToken);

    Task ProcessFailedReset(string data, string errorMessage, CancellationToken cancellationToken);
}
