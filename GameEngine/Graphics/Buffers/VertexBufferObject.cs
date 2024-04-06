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

    public static VertexBufferObject FromBufferLayout(BufferLayout bufferLayout, BufferUsageHint bufferUsageHint, int predefinedVertexCount)
    {
        var vbo = new VertexBufferObject(predefinedVertexCount * bufferLayout.Stride, bufferUsageHint)
        {
            BufferLayout = bufferLayout
        };

        return vbo;
    }

    public void SetData<T>(List<T> data, BufferUsageHint? bufferUsageHint = null) where T : struct
    {
        Bind();

        if (bufferUsageHint.HasValue)
        {
            BufferUsageHint = bufferUsageHint.Value;
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Marshal.SizeOf(typeof(T)), data.ToArray(), BufferUsageHint);
        }
        else
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, data.Count * Marshal.SizeOf(typeof(T)), data.ToArray());
        }
    }

    public void SetData<T>(IEnumerable<T> data, BufferUsageHint? bufferUsageHint = null) where T : struct
    {
        Bind();

        if (bufferUsageHint.HasValue)
        {
            BufferUsageHint = bufferUsageHint.Value;
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count() * Marshal.SizeOf(typeof(T)), data.ToArray(), BufferUsageHint);
        }
        else
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, data.Count() * Marshal.SizeOf(typeof(T)), data.ToArray());
        }
    }

    public void SetData<T>(T[] data, BufferUsageHint? bufferUsageHint = null) where T : struct
    {
        Bind();

        if (bufferUsageHint.HasValue)
        {
            BufferUsageHint = bufferUsageHint.Value;
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Marshal.SizeOf(typeof(T)), data, BufferUsageHint);
        }
        else
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, data.Length * Marshal.SizeOf(typeof(T)), data);
        }
    }

    public void SetData<T>(T[] data, int offset) where T : struct
    {
        Bind();
        GL.BufferSubData(BufferTarget.ArrayBuffer, offset, data.Length * Marshal.SizeOf(typeof(T)), data);
    }

    public void SetData<T>(T data, int offset) where T : struct
    {
        Bind();
        GL.BufferSubData(BufferTarget.ArrayBuffer, offset, Marshal.SizeOf(typeof(T)), ref data);
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
