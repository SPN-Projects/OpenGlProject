using System.Runtime.InteropServices;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;

public class UniformBuffer : Buffer
{
    public new BufferUsageHint BufferUsageHint { get; protected set; }
    public new int Handle { get; protected set; }

    public UniformBuffer(int size, int binding, BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicDraw)
    {
        BufferUsageHint = bufferUsageHint;
        Handle = GL.GenBuffer();

        Bind();
        GL.BufferData(BufferTarget.UniformBuffer, size, nint.Zero, bufferUsageHint);
        Unbind();

        LinkToBinding(binding);

        Logger.EngineLogger.Trace($"Created Uniform Buffer [{Handle}] with size [{size}] and binding [{binding}]");
    }

    public UniformBuffer(int size, int binding, int offset, int range, BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicDraw)
    {
        BufferUsageHint = bufferUsageHint;
        Handle = GL.GenBuffer();

        Bind();
        GL.BufferData(BufferTarget.UniformBuffer, size, nint.Zero, bufferUsageHint);
        Unbind();

        LinkToBindingRange(binding, offset, range);

        Logger.EngineLogger.Trace($"Created Uniform Buffer [{Handle}] with size [{size}], binding [{binding}], offset [{offset}], and range [{range}]");
    }

    public void SetData<T>(T data, int offset = 0) where T : struct
    {
        Bind();
        GL.BufferSubData(BufferTarget.UniformBuffer, offset, Marshal.SizeOf(data), ref data);
    }

    public override void Bind()
        => GL.BindBuffer(BufferTarget.UniformBuffer, Handle);
    public override void Unbind()
        => GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    private void LinkToBinding(int binding)
        => GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, Handle);

    private void LinkToBindingRange(int binding, int offset, int size)
        => GL.BindBufferRange(BufferRangeTarget.UniformBuffer, binding, Handle, offset, size);

    public override void Dispose()
    {
        Logger.EngineLogger.Trace($"Disposing Uniform Buffer [{Handle}]");
        GL.DeleteBuffer(Handle);
        GC.SuppressFinalize(this);
    }
}