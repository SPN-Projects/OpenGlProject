using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Graphics.Buffers;
public class VertexArrayObject : Buffer
{
    private int _currentLayoutIndex;
    public List<VertexBufferObject> VertexBufferObjects { get; }
    public ElementBufferObject? ElementBufferObject { get; private set; }

    public VertexArrayObject()
    {
        VertexBufferObjects = [];
        Handle = GL.GenVertexArray();
        _currentLayoutIndex = 0;
    }

    public void AddVertexBufferObject(VertexBufferObject vbo)
    {
        Bind();
        vbo.Bind();

        if (vbo.BufferLayout?.Elements is null or { Count: 0 })
        {
            throw new InvalidOperationException("VertexBufferObject must have a BufferLayout with at least one element.");
        }

        foreach (var element in vbo.BufferLayout)
        {
            GL.VertexAttribPointer(_currentLayoutIndex, element.ComponentCount, element.Type, element.Normalized, vbo.BufferLayout.Stride, element.Offset);
            GL.EnableVertexAttribArray(_currentLayoutIndex);
            if (element.Divisor > 0)
            {
                GL.VertexAttribDivisor(_currentLayoutIndex, element.Divisor);
            }


            _currentLayoutIndex++;
        }

        VertexBufferObjects.Add(vbo);
    }

    public void SetElementBufferObject(ElementBufferObject ebo)
    {
        Bind();
        ebo.Bind();

        ElementBufferObject = ebo;
    }

    public override void Bind()
        => GL.BindVertexArray(Handle);
    public override void Unbind()
        => GL.BindVertexArray(0);
    public override void Dispose()
    {
        GL.DeleteVertexArray(Handle);
        GC.SuppressFinalize(this);
    }
}
