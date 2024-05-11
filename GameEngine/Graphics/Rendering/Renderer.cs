using System.Runtime.InteropServices;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Textures;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine.Graphics.Rendering;

public static class Renderer
{
    internal static RenderData? RenderData { get; private set; }

    internal static void Init()
    {
        RenderData = new RenderData
        {
            CameraUniformBuffer = new UniformBuffer(Marshal.SizeOf(typeof(CameraData)), 0),
            _currentClearBufferMasks = [],
            _currentEnableCaps = [],
        };
    }

    public static void BeginScene(Camera camera)
    {
        if (RenderData is not null and { _isCleared: false })
        {
            Logger.EngineLogger.Warning("Scene has not been cleared.");
            throw new InvalidOperationException("Scene has not been cleared.");
        }

        if (camera is null || RenderData?.CameraUniformBuffer is null)
        {
            return;
        }

        RenderData.CameraBuffer = new CameraData
        {
            CameraPosition = new Vector4(camera.Position),
            ProjectionMatrix = camera.Projection,
        };

        RenderData.CameraUniformBuffer.SetData(RenderData.CameraBuffer);

        ResetBatch();
    }

    public static void EndScene()
        => Flush();

    public static void FlushAndReset()
    {
        EndScene();
        ResetBatch();
    }

    private static void Flush()
    {
        if (RenderData!.IndexCount <= 0)
        {
            return;
        }
    }

    private static void ResetBatch()
    {
        RenderData!.VertexCount = 0;
        RenderData!.IndexCount = 0;

        RenderData._isCleared = false;
    }

    public static void Enable(params EnableCap[] enableCap)
    {
        if(enableCap.Any(ec => RenderData!._currentEnableCaps.Contains(ec)))
            throw new Exception($"EnableCap {enableCap} was tried to be enabled, but was already enabled before!");

        RenderData!._currentEnableCaps.AddRange(RenderData._currentEnableCaps.Except(enableCap));
    }

    public static void Disable(params EnableCap[] enableCaps)
    {
        foreach(var enableCap in enableCaps)
        {
            if (RenderData!._currentEnableCaps.Contains(enableCap))
                throw new Exception($"EnableCap {enableCap} was tried to be removed, but was not enabled before!");

            RenderData!._currentEnableCaps.Remove(enableCap);
        }

        UpdateEnableCap();
    }

    private static void UpdateEnableCap()
    {
        GL.Enable(RenderData!._currentEnableCaps.Aggregate((currentEnableCap, nextEnableCap) => currentEnableCap | nextEnableCap));
    }

    public static void ClearScene(params ClearBufferMask[] clearBufferMask)
    {
        if (RenderData is null)
        {
            Logger.EngineLogger.Warning("RenderData is not initialized.");
            throw new InvalidOperationException("RenderData is not initialized.");
        }

        var combinedMask = clearBufferMask.Aggregate((currentMask, nextMask) => currentMask | nextMask);

        GL.Clear(combinedMask);
        RenderData._isCleared = true;
    }

    public static void ClearColor(float r, float g, float b, float a)
    {
        GL.ClearColor(r, g, b, a);
    }
}
