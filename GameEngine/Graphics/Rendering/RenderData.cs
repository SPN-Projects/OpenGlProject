using GameEngine.Graphics.Buffers;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Rendering;

internal class RenderData
{
    public const int MaxTextureSlots = EngineConstants.MaxTextureSlots;
    public const int MaxElementCount = EngineConstants.MaxElementCount;

    public const int MaxQuadVertices = MaxElementCount * 4;
    public const int MaxQuadIndices = MaxElementCount * 6;

    public const int MaxLineVertices = MaxElementCount * 2;
    public const int MaxLineIndices = MaxElementCount * 6;

    public const int MaxIndices = MaxQuadIndices + MaxLineIndices;

    public int VertexCount { get; internal set; }
    public int IndexCount { get; internal set; }

    internal bool _isCleared = false;

    internal required List<ClearBufferMask> _currentClearBufferMasks { get; set; }
    internal required List<EnableCap> _currentEnableCaps { get; set; }


    // Camera
    public CameraData CameraBuffer { get; internal set; }
    public required UniformBuffer CameraUniformBuffer { get; internal set; }

    public RenderData()
    {
        CameraBuffer = new CameraData();
    }
}