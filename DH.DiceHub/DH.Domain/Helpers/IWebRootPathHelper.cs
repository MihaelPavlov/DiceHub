namespace DH.Domain.Helpers;

/// <summary>
/// Interface for providing access to the web root path of a hosting environment.
/// This is typically implemented using <see cref="IWebHostEnvironment"/> to retrieve the 
/// 'wwwroot' directory for a web application.
/// </summary>
public interface IWebRootPathHelper
{
    /// <summary>
    /// Gets the web root path (the 'wwwroot' directory) of the hosting environment.
    /// </summary>
    /// <value>A string representing the web root path.</value>
    string GetWebRootPath { get; }
}
