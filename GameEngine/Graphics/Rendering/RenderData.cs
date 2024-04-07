﻿using GameEngine.Graphics.Buffers;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Rendering;

internal class RenderData
{
    public const int MaxTextureSlots = EngineConstants.MaxTextureSlots;
    public const int MaxElementCount = EngineConstants.MaxElementCount;

    public int VertexCount { get; internal set; }
    public int IndexCount { get; internal set; }

    internal bool _isCleared = false;
    internal ClearBufferMask _clearBufferMask;

    // Camera
    public CameraData CameraBuffer { get; internal set; }
    public UniformBuffer? CameraUniformBuffer { get; internal set; }

    public RenderData()
    {
        CameraBuffer = new CameraData();
    }
}