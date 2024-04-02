using GameEngine;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Rendering;
using GameEngine.Graphics.Shaders;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Test.Graphics;

namespace Test;
public class TestGame : Game, IDisposable
{
    private readonly Random _random = new();
    private QuadBatch? _quadBatch;

    private PerspectiveCamera _camera;

    private bool _disposedValue;

    public TestGame(string title, GameWindowSettings? gameWindowSettings = null, NativeWindowSettings? nativeWindowSettings = null) : base(title, gameWindowSettings, nativeWindowSettings)
    {
        Run();
    }

    protected override void OnLoad()
    {
        Logger.GameLogger.Info("Loading Game...");

        ToggleWindowVisibility(true);

        ToggleFullscreen(Program.GameConfig!.Fullscreen);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _quadBatch = new QuadBatch(10000, Shader.Default);

        for (var i = 0; i < 10000; i++)
        {
            var position = new Vector3(_random.Next(-100, 100), _random.Next(-100, 100), _random.Next(-100, 100));
            var size = new Vector2(_random.Next(1, 10), _random.Next(1, 10));
            var color = new Vector4(_random.Next(0, 255) / 255f, _random.Next(0, 255) / 255f, _random.Next(0, 255) / 255f, 1.0f);

            _quadBatch.Quads.Add(new Quad(position, size, color));
        }

        _camera = new PerspectiveCamera(90f, (float)NativeWindow.ClientSize.X / NativeWindow.ClientSize.Y, 0.1f, 1000.0f)
        {
            Position = new Vector3(0, 0, 2)
        };
    }

    protected override void OnUnload()
    {
        Logger.GameLogger.Info("Unloading Game...");

        Dispose();
    }

    protected override void Update(double deltaTime)
    {
        var speed = 6f;
        var shiftSpeed = 4f;
        var direction = Vector3.Zero;

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
        {
            direction += new Vector3(0, 1, 0);
        }

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.C))
        {
            direction += new Vector3(0, -1, 0);
        }

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
        {
            direction += new Vector3(-1, 0, 0);
        }

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
        {
            direction += new Vector3(1, 0, 0);
        }

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            direction += new Vector3(0, 0, -1);
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            direction += new Vector3(0, 0, 1);
        }

        if (direction != Vector3.Zero)
        {
            direction.Normalize();
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            direction *= shiftSpeed;
        }


        _camera.Position += direction * (float)(speed * deltaTime);


        // Zooming
        if (MouseState.ScrollDelta.Y != 0)
        {
            _camera.FieldOfView -= MouseState.ScrollDelta.Y * 1f;
        }
    }

    protected override void Render(double deltaTime)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Renderer.BeginScene(_camera);

        _quadBatch!.Draw(_camera);

        Renderer.EndScene();
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
                _quadBatch?.Dispose();
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
