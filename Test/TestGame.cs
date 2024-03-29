using GameEngine;
using GameEngine.Graphics;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;

namespace Test;
public class TestGame : Game, IDisposable
{
    private Shader? _shader;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private bool _disposedValue;

    public TestGame(string title, GameWindowSettings? gameWindowSettings = null, NativeWindowSettings? nativeWindowSettings = null) : base(title, gameWindowSettings, nativeWindowSettings)
    {
        Run();
    }

    protected override void OnLoad()
    {
        Logger.GameLogger.Info("Loading Game...");

        ToggleWindowVisibility(true);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // create shader
        _shader = Shader.Default;

        // init triangle vertices
        float[] vertices =
        [
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0.0f, 0.5f, 0.0f
        ];

        // create vertex buffer object -> we need to create a buffer object to store the vertices in the GPU memory
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // create vertex array object -> we need to create a vertex array object to store the vertex attribute pointers
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    protected override void OnUnload()
    {
        Logger.GameLogger.Info("Unloading Game...");

        Dispose();
    }

    protected override void Update(double deltaTime)
    {
    }

    protected override void Render(double deltaTime)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader?.Use();
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        SwapBuffers();
    }

    protected override void OnResize(int width, int height)
        => GL.Viewport(0, 0, width, height);

    protected override void OnExit()
        => Logger.GameLogger.Info("Exiting Game...");

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _shader?.Dispose();

                Logger.GameLogger.Info("Unloading Buffer Object");
                GL.DeleteBuffer(_vertexBufferObject);

                Logger.GameLogger.Info("Unloading Vertex Array Object");
                GL.DeleteVertexArray(_vertexArrayObject);
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
