using System.Runtime.InteropServices;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine.Graphics.Rendering;

public static class Renderer
{
    public static RenderData? RenderData { get; private set; }
    public static void Init(EnableCap? enableCaps = null)
    {
        if (enableCaps.HasValue)
        {
            GL.Enable(enableCaps.Value);
        }

        RenderData = new RenderData
        {
            CameraUniformBuffer = new UniformBuffer(Marshal.SizeOf(typeof(CameraData)), 0)
        };
    }

    public static void BeginScene(Camera camera)
    {
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
    }
}
