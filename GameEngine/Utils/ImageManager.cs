using GameEngine.Logging;
using StbImageSharp;

namespace GameEngine.Utils;
public class ImageManager : ResourceManager<ImageResult>
{
    public static ImageResult LoadImage(string path, ColorComponents colorComponents = ColorComponents.RedGreenBlueAlpha, bool flipVertically = false)
    {
        PathChecker.EnsureFile(path);
        var uniqueName = $"{path}_{colorComponents}_{flipVertically}";

        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

        var image = Instance.GetResource(uniqueName) ?? Instance.AddResource(uniqueName, ImageResult.FromStream(FileManager.OpenFile(path), colorComponents));
        if (image == null)
        {
            Logger.EngineLogger.Error($"Failed to load image from {path}");
            throw new Exception("Failed to load image");
        }

        return image;
    }
}
