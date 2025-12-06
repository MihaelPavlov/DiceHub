namespace DH.Domain.Adapters.Statistics;

public class StatisticJobQueueHelper
{
    public static string BuildJobId(string typeOfJob, string userId) => $"{typeOfJob}-{userId}";
}
