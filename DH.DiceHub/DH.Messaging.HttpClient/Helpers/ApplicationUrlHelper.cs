using DH.Messaging.HttpClient.Enums;

namespace DH.Messaging.HttpClient.Helpers;

public static class ApplicationUrlHelper
{
    public static string GetApplicationUrl(ApplicationApi application)
    {
        return application switch
        {
            ApplicationApi.Statistics => $"https://statistics",
        };
    }
}
