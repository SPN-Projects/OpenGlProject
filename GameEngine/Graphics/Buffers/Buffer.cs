using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;
public abstract class Buffer : IDisposable
{
    public BufferUsageHint BufferUsageHint { get; protected set; }
    public int Handle { get; protected set; }

    public abstract void Bind();
    public abstract void Unbind();
    public abstract void Dispose();
}
