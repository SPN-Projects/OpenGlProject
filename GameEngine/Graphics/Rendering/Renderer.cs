using System.Runtime.InteropServices;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;

namespace GameEngine.Graphics.Rendering;

public static class Renderer
{
    public static RenderData? RenderData { get; private set; }
    public static void Init() => RenderData = new RenderData
    {
        CameraUniformBuffer = new UniformBuffer(Marshal.SizeOf(typeof(CameraData)), 0)
    };

    public static void BeginScene(Camera camera)
    {
        if (camera is null || RenderData?.CameraUniformBuffer is null)
        {
            return;
        }

        RenderData.CameraBuffer = new CameraData
        {
            ViewProjectionMatrix = camera.ViewProjectionMatrix
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
    }
}
