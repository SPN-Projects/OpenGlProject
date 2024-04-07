using GameEngine.Utils;

namespace GameEngine.Graphics.Textures;
public class TextureManager : ResourceManager<Texture>
{
    public static Texture Default
        => Instance.GetResource(EngineConstants.DefaultTexturePath) ?? Instance.AddResource(EngineConstants.DefaultTexturePath, Texture.FromFile(EngineConstants.DefaultTexturePath));
}
