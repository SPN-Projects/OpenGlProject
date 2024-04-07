namespace GameEngine.Utils;
internal class FileManager : ResourceManager<FileStream>
{
    public static FileStream OpenFile(string path)
    {
        PathChecker.EnsureFile(path);
        return Instance.GetResource(path) ?? Instance.AddResource(path, File.OpenRead(path));
    }
}
