using GameEngine.Graphics.Buffers;

namespace GameEngine.Graphics.Rendering;

public class RenderData
{
    public const int MaxTextureSlots = EngineConstants.MaxTextureSlots;
    public const int MaxElementCount = EngineConstants.MaxElementCount;

    public int VertexCount { get; internal set; }
    public int IndexCount { get; internal set; }


    // Camera
    public CameraData CameraBuffer { get; internal set; }
    public UniformBuffer? CameraUniformBuffer { get; internal set; }

    public RenderData()
    {
        CameraBuffer = new CameraData();
    }
}