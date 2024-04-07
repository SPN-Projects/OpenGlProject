using GameEngine.Utils;

namespace GameEngine.Graphics.Textures;
public class TextureManager : ResourceManager<Texture>
{
    public static Texture2D Default2D
        => (Texture2D)(Instance.GetResource(EngineConstants.DefaultTexturePath) ?? Instance.AddResource(EngineConstants.DefaultTexturePath, new Texture2D(EngineConstants.DefaultTexturePath)));

    public static Texture2D WhiteTexture2D
        => (Texture2D)(Instance.GetResource(EngineConstants.WhiteTextureName) ?? Instance.AddResource(EngineConstants.WhiteTextureName, Texture2D.White));
}
