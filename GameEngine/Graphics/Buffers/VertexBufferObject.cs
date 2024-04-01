using System.Runtime.InteropServices;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;
public class VertexBufferObject : Buffer
{
    public int Count { get; }
    public BufferLayout? BufferLayout { get; private set; }

    public VertexBufferObject(int size, BufferUsageHint bufferUsageHint)
    {
        BufferUsageHint = bufferUsageHint;
        Count = size;

        Handle = GL.GenBuffer();
        Bind();

        // allocate memory for the buffer
        GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, bufferUsageHint);

        Logger.EngineLogger.Trace($"Created Vertex Buffer Object [{Handle}] with size [{size}]");
    }

    public VertexBufferObject(float[] vertices, int size, BufferUsageHint bufferUsageHint)
    {
        BufferUsageHint = bufferUsageHint;
        Count = size;

        Handle = GL.GenBuffer();
        Bind();

        // allocate memory for the buffer
        GL.BufferData(BufferTarget.ArrayBuffer, size, vertices, bufferUsageHint);

        Logger.EngineLogger.Trace($"Created Vertex Buffer Object [{Handle}] with size [{size}]");
    }

    public void SetData<T>(List<T> data) where T : struct
    {
        Bind();
        GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Marshal.SizeOf(typeof(T)), data.ToArray(), BufferUsageHint);
    }

    public void SetBufferLayout(BufferLayout quadLayout)
        => BufferLayout = quadLayout;

    public override void Bind()
        => GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
    public override void Unbind()
        => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    public override void Dispose()
        => GL.DeleteBuffer(Handle);
}
