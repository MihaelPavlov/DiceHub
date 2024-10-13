namespace DH.Api.Helpers;

/// <summary>
/// Provides functionality to build the path to the 'wwwroot' directory within the 'DiceHub' project.
/// This class constructs the web root path based on the base directory of the current application and
/// ensures it locates the 'DiceHub' segment in the path to form the correct folder structure.
/// Throws an <see cref="InvalidOperationException"/> if 'DiceHub' is not found in the path.
/// </summary>
public static class ProjectPathHelper
{
    /// <summary>
    /// Constructs the path to the 'wwwroot' directory of the 'DiceHub' project by searching for the 
    /// 'DiceHub' segment in the base directory path. Replaces the original path with the 
    /// specific folder structure required.
    /// Throws an <see cref="InvalidOperationException"/> if the 'DiceHub' segment is not found.
    /// </summary>
    /// <returns>The resolved 'wwwroot' path for the 'DiceHub' project.</returns>
    public static string BuildWebRootPath()
    {
        var baseDirectory = AppContext.BaseDirectory;

        // Construct the path to wwwroot
        var webRootPath = Path.Combine(baseDirectory, "wwwroot");

        // Find the index of "DiceHub" in the original path
        int diceHubIndex = webRootPath.IndexOf("DiceHub");

        // Check if "DiceHub" was found in the path
        if (diceHubIndex == -1)
            throw new InvalidOperationException("The word 'DiceHub' was not found in the original path.");

        // Construct the new path starting from "DiceHub"
        string newFolder = @"DiceHub\DH.DiceHub\DH.Api\wwwroot";
        string newPath = webRootPath.Substring(0, diceHubIndex) + newFolder;

        return newPath;
    }
}
