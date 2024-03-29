namespace GameEngine.Utils;

internal class PathChecker
{
    /// <summary>
    /// Checks whether the file exists at the given path.
    /// </summary>
    /// <param name="path">The given path</param>
    /// <exception cref="FileNotFoundException">Thrown when the file is not found</exception>
    public static void EnsureFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found at {path}");
        }
    }

    /// <summary>
    /// Checks whether the directory exists at the given path.
    /// </summary>
    /// <param name="path">The given path</param>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory is not found</exception>
    public static void EnsurePath(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found at {path}");
        }
    }
}