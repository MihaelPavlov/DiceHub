using DH.Domain.Helpers;

namespace DH.Api.Helpers;

/// <inheritdoc/>
public class WebRootPathHelper : IWebRootPathHelper
{
    readonly IWebHostEnvironment hostEnvironment;

    public WebRootPathHelper(IWebHostEnvironment hostEnvironment)
    {
        this.hostEnvironment = hostEnvironment;
    }

    /// <inheritdoc
    public string GetWebRootPath => this.hostEnvironment.WebRootPath;
}
