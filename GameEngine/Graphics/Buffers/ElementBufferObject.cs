using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;

public class ElementBufferObject : Buffer
{
    public int Count { get; }

    public ElementBufferObject(uint[] indices, int count, BufferUsageHint bufferUsageHint)
    {
        BufferUsageHint = bufferUsageHint;
        Count = count;

        Handle = GL.GenBuffer();
        Bind();

        // allocate memory for the buffer
        GL.BufferData(BufferTarget.ElementArrayBuffer, count * sizeof(uint), indices, bufferUsageHint);

        Logger.EngineLogger.Trace($"Created Element Buffer Object [{Handle}] with size [{count}]");
    }

    public void SetData(dynamic data, int size)
        => GL.BufferData(BufferTarget.ElementArrayBuffer, size, data, BufferUsageHint);
    public override void Bind()
        => GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
    public override void Unbind()
        => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    public override void Dispose()
    {
        GL.DeleteBuffer(Handle);
        GC.SuppressFinalize(this);
    }
}