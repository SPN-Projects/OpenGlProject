using GameEngine;
using GameEngine.Extensions;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Rendering;
using GameEngine.Graphics.Shaders;
using GameEngine.Graphics.Textures;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Test.Graphics;

namespace Test;
public class TestGame : Game, IDisposable
{
    private bool _disposedValue;
    private readonly Random _random = new();
    private readonly List<QuadBatch> _quadBatches = [];

    private Quad? _playerQuad;

    private PerspectiveCamera? _camera;

    private readonly int _quadCount = 1000000;
    private readonly Vector2 _scenePlaneSize = new(1000, 1000);

    public TestGame(string title, GameWindowSettings? gameWindowSettings = null, NativeWindowSettings? nativeWindowSettings = null) : base(title, gameWindowSettings, nativeWindowSettings)
    {
        Run();
    }

    protected override void OnLoad()
    {
        Logger.GameLogger.Info("Loading Game...");

        ToggleWindowVisibility(true);

        ToggleFullscreen(Program.GameConfig!.Fullscreen);

        Renderer.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        Renderer.Enable(EnableCap.DepthTest, EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        var sceneQuadBatch = new QuadBatch(_quadCount, ShaderManager.Default, TextureManager.Default2D);
        var playersQuadBatch = new QuadBatch(1, ShaderManager.Default, TextureManager.Default2D);

        var scenePlaneQuad = new Quad(new Vector3(0, 0, 0), _scenePlaneSize, new Vector4(1.0f));
        sceneQuadBatch.AddQuad(scenePlaneQuad);

        _playerQuad = new Quad(new Vector3(0, 0, 1), new Vector2(10, 10), new Vector4(1.0f, 0.0f, 0.0f, 0.5f));
        playersQuadBatch.AddQuad(_playerQuad);

        playersQuadBatch.RecalculateAll();
        sceneQuadBatch.RecalculateAll();
        _quadBatches.AddRange(sceneQuadBatch, playersQuadBatch);

        _camera = new PerspectiveCamera(90f, (float)NativeWindow.ClientSize.X / NativeWindow.ClientSize.Y, 0.1f, 100000.0f)
        {
            Position = new Vector3(0, 0, 50)
        };
    }

    protected override void OnUnload()
        => Logger.GameLogger.Info("Unloading Game...");

    protected override void Update(double deltaTime)
    {
        var speed = 20;
        var shiftSpeed = 40;
        var direction = Vector3.Zero;

        if (KeyboardState.IsKeyPressed(Keys.F11))
        {
            ToggleFullscreen(!NativeWindow.IsFullscreen);
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            direction += new Vector3(-1, 0, 0);
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            direction += new Vector3(1, 0, 0);
        }

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            direction += new Vector3(0, 1, 0);
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            direction += new Vector3(0, -1, 0);
        }

        if (direction != Vector3.Zero)
        {
            direction.Normalize();
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            speed *= shiftSpeed;
        }

        _playerQuad!.Position += direction * (float)(speed * deltaTime);
        _camera!.Position = _playerQuad.Position + new Vector3(0, 0, 100);

        // Zooming
        if (MouseState.ScrollDelta.Y != 0)
        {
            _camera.FieldOfView -= MouseState.ScrollDelta.Y * 1f;
        }
    }

    protected override void Render(double deltaTime)
    {
        Renderer.ClearScene(ClearBufferMask.ColorBufferBit, ClearBufferMask.DepthBufferBit);
        Renderer.BeginScene(_camera!);

        foreach (var quadBatch in _quadBatches)
        {
            quadBatch!.Draw(_camera);
        }

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
                foreach (var quadBatch in _quadBatches)
                {
                    quadBatch?.Dispose();
                }
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
