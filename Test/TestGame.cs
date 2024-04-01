using GameEngine;
using GameEngine.Graphics;
using GameEngine.Graphics.Shapes;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Test;
public class TestGame : Game, IDisposable
{
    private Shader? _shader;
    private bool _disposedValue;
    private SpriteBatch _triangleSpriteBatch;
    private List<Triangle> _shapes = new();

    public TestGame(string title) : base(title)
    {
        Run();
    }

    protected override void OnLoad()
    {
        Logger.GameLogger.Info("Loading Game...");
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // create shader
        _shader = Shader.Default;
        _triangleSpriteBatch = new SpriteBatch();
        _triangleSpriteBatch.Load();

        CreateTestingShapes();

        _triangleSpriteBatch.Add(_shapes.First());
        //_triangleSpriteBatch.Add(_shapes[1]);
    }

    private void CreateTestingShapes()
    {
        _shapes.Add(new Triangle([
            -0.5f,
            -0.5f,
            0.0f, //Bottom-left vertex
            0.5f,
            -0.5f,
            0.0f, //Bottom-right vertex
            0.0f,
            0.5f,
            0.0f  //Top vertex
            ]));

        _shapes.Add(new Triangle([
            0.0f,
            0.0f,
            0.0f,
            0.5f,
            0.0f,
            0.0f,
            0.0f,
            0.5f,
            0.0f
        ]));

        _shapes.Add(new Triangle([
            -0.5f,
            -0.5f,
            0.0f,
            0.0f,
            -0.5f,
            0.0f,
            0.0f,
            0.0f,
            0.0f
        ]));

        _shapes.Add(new Triangle([ // something went wrong while scaling and those were the vertice data but somehow they seems to work -> further investigation potential
            -0.53f,
            - 0.52f,
            0.0f,
            0.53f,
            -0.52f,
            0.0f,
            0.0f,
            0.539999962f,
            0.0f
        ]));
    }

    protected override void OnUnload()
        => Logger.GameLogger.Info("Unloading Game...");

    protected override void Update(double deltaTime)
    {
        // Test size Increase
        if (_keyboardState.IsKeyDown(Keys.Down))
        {
            _shapes.ForEach(s => s.Size -= 0.01f);
            _triangleSpriteBatch.UpdateData();
        }

        if (_keyboardState.IsKeyDown(Keys.Up))
        {
            _shapes.ForEach(s => s.Size += 0.01f);
            _triangleSpriteBatch.UpdateData();
        }
    }

    protected override void Render(double deltaTime)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader?.Use();
        _triangleSpriteBatch.Draw();

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
                _triangleSpriteBatch?.Dispose();
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
