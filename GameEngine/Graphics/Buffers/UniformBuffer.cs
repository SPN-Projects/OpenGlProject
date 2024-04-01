using System.Runtime.InteropServices;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;

public class UniformBuffer : IDisposable
{
    public BufferUsageHint BufferUsageHint { get; protected set; }
    public int Handle { get; protected set; }

    public UniformBuffer(int size, int binding, BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw)
    {
        BufferUsageHint = bufferUsageHint;
        Handle = GL.GenBuffer();

        GL.NamedBufferData(Handle, size, nint.Zero, bufferUsageHint);
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, Handle);

        Logger.EngineLogger.Info($"Created Uniform Buffer [{Handle}] with size [{size}] and binding [{binding}]");
    }

    public void SetData<T>(T data, int offset = 0) where T : struct
        => GL.NamedBufferSubData(Handle, offset, Marshal.SizeOf(data), ref data);

    public void Dispose()
        => GL.DeleteBuffer(Handle);
}