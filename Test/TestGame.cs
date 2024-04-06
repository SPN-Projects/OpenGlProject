using GameEngine;
using GameEngine.Extensions;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Rendering;
using GameEngine.Graphics.Shaders;
using GameEngine.Graphics.Textures;
using GameEngine.Logging;
using GameEngine.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Test.Graphics;

namespace Test;
public class TestGame : Game, IDisposable
{
    private readonly Random _random = new();
    private readonly List<QuadBatch> _quadBatches = [];

    private PerspectiveCamera? _camera;

    private bool _disposedValue;
    private bool _isInterpolating;
    private readonly int _quadCount = 4;
    private readonly int _quadBatchCount = 1;
    private readonly int _spawnDistance = 10;
    private readonly int _endDistance = 1;
    private readonly int _randomQuadPercentage = 1;

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

        for (var i = 0; i < _quadBatchCount; i++)
        {
            var quadBatch = new QuadBatch(_quadCount, Shader.Default, Texture.Default);

            var quadsBatchColor = new Vector4(_random.NextSingle(), _random.NextSingle(), _random.NextSingle(), 1.0f);
            for (var j = 0; j < _quadCount; j++)
            {
                var position = new Vector3(_random.Next(-_spawnDistance, _spawnDistance), _random.Next(-_spawnDistance, _spawnDistance), _random.Next(-_spawnDistance, _spawnDistance));
                var size = new Vector2(_random.Next(1, 2), _random.Next(1, 2));

                quadBatch.AddQuad(new Quad(position, size, quadsBatchColor));
            }

            quadBatch.RecalculateAll();
            _quadBatches.Add(quadBatch);
        }

        _camera = new PerspectiveCamera(90f, (float)NativeWindow.ClientSize.X / NativeWindow.ClientSize.Y, 0.1f, 100000.0f)
        {
            Position = new Vector3(0, 0, 2)
        };
    }

    protected override void OnUnload()
        => Logger.GameLogger.Info("Unloading Game...");

    protected override void Update(double deltaTime)
    {
        var speed = 6;
        var superInterpolationSpeed = 100;
        var shiftSpeed = 40;
        var direction = Vector3.Zero;
        var randomCubeCount = _quadCount / 100 * _randomQuadPercentage;

        if (KeyboardState.IsKeyPressed(Keys.F11))
        {
            ToggleFullscreen(!NativeWindow.IsFullscreen);
        }

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
            speed *= shiftSpeed;
            randomCubeCount *= superInterpolationSpeed;
        }

        if (KeyboardState.IsKeyPressed(Keys.Enter))
        {
            _isInterpolating = !_isInterpolating;
        }

        _camera!.Position += direction * (float)(speed * deltaTime);


        // Zooming
        if (MouseState.ScrollDelta.Y != 0)
        {
            _camera.FieldOfView -= MouseState.ScrollDelta.Y * 1f;
        }

        if (_isInterpolating)
        {
            InterpolateRandomQuads(randomCubeCount, Vector3.Zero, _endDistance);
        }
    }

    private void InterpolateRandomQuads(int randomQuadCount, Vector3 cubePosition, int cubeSize)
    {
        const float interpolationSpeed = 0.55f;

        _ = Parallel.ForEach(_quadBatches, (quadBatch) =>
        {
            var randomQuads = quadBatch!.Quads.TakeRandomWithIndex(randomQuadCount);

            _ = Parallel.ForEach(randomQuads, (randomQuad) =>
            {
                var squareDistanceToCube = SPNMath.SquareDistance(randomQuad.quad.Position, cubePosition);
                var isNotInsideCube = squareDistanceToCube > Math.Pow(cubeSize, 2);
                if (isNotInsideCube)
                {
                    randomQuad.quad.Position = SPNMath.Lerp(randomQuad.quad.Position, Vector3.Zero, interpolationSpeed);
                    randomQuad.quad.IsVisible = true;
                    randomQuad.quad.Recalculate();

                    if (!quadBatch.InvalidatedQuads.ContainsKey(randomQuad.index))
                    {
                        _ = quadBatch.InvalidatedQuads.TryAdd(randomQuad.index, randomQuad.quad);
                    }
                    else
                    {
                        quadBatch.InvalidatedQuads[randomQuad.index] = randomQuad.quad;
                    }
                }
            });
        });
    }

    protected override void Render(double deltaTime)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Renderer.BeginScene(_camera!);

        foreach (var quadBatch in _quadBatches)
        {
            quadBatch!.Draw(_camera, [.. quadBatch.InvalidatedQuads]);
            quadBatch.InvalidatedQuads.Clear();
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
