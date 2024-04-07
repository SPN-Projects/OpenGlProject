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

    public static void Init(Vector4 clearColor, ClearBufferMask clearBufferMask, EnableCap? enableCaps = null)
    {
        if (enableCaps.HasValue)
        {
            GL.Enable(enableCaps.Value);
        }

        GL.ClearColor(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);

        RenderData = new RenderData
        {
            CameraUniformBuffer = new UniformBuffer(Marshal.SizeOf(typeof(CameraData)), 0),
            _clearBufferMask = clearBufferMask,
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

    public static void DrawSprite(Sprite sprite, Vector3 position, Vector2 size, Vector4 color)
    {

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

    public static void ClearScene()
    {
        if (RenderData is null)
        {
            Logger.EngineLogger.Warning("RenderData is not initialized.");
            throw new InvalidOperationException("RenderData is not initialized.");
        }

        GL.Clear(RenderData._clearBufferMask);
        RenderData._isCleared = true;
    }
}
