using GameEngine.Utils;

namespace GameEngine.Graphics.Shaders;
public class ShaderManager : ResourceManager<Shader>
{
    public static Shader Default
        => Instance.GetResource(EngineConstants.DefaultShaderName) ?? Instance.AddResource(EngineConstants.DefaultShaderName, Shader.FromName(EngineConstants.DefaultShaderName));
}
