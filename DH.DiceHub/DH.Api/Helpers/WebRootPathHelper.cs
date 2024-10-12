using DH.Domain.Helpers;

namespace DH.Api.Helpers;

public class WebRootPathHelper : IWebRootPathHelper
{
    readonly IWebHostEnvironment hostEnvironment;

    public WebRootPathHelper(IWebHostEnvironment hostEnvironment)
    {
        this.hostEnvironment = hostEnvironment;
    }

    public string GetWebRootPath => this.hostEnvironment.WebRootPath;
}
