using GameEngine;
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
    private readonly int _quadCount = 100000;
    private readonly int _quadBatchCount = 3;
    private readonly int _spawnDistance = 10000;
    private readonly int _endDistance = 50;
    private readonly int _randomQuadPercentage = 2;
    private readonly float _interpolationSpeed = 0.5f;

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
            var quadBatch = new QuadBatch(_quadCount, ShaderManager.Default, TextureManager.Default);

            var quadsBatchColor = new Vector4(_random.NextSingle(), _random.NextSingle(), _random.NextSingle(), 1.0f);
            for (var j = 0; j < _quadCount; j++)
            {
                var position = new Vector3(_random.Next(-_spawnDistance, _spawnDistance), _random.Next(-_spawnDistance, _spawnDistance), _random.Next(-_spawnDistance, _spawnDistance));
                var size = new Vector2(_random.Next(1, 10), _random.Next(1, 10));
                var rotation = new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle());

                var newQuad = new Quad(position, size, quadsBatchColor)
                {
                    Rotation = rotation
                };

                quadBatch.AddQuad(newQuad);
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
        var superInterpolationSpeed = 10;
        var shiftSpeed = 40;
        var direction = Vector3.Zero;

        _ = _quadCount / 100 * _randomQuadPercentage;
        var interpolationSpeed = _interpolationSpeed;

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
            interpolationSpeed *= superInterpolationSpeed;
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
            //InterpolateRandomQuads(randomCubeCount, interpolationSpeed, Vector3.Zero, _endDistance);
            InterpolateAllQuads(interpolationSpeed, Vector3.Zero, _endDistance, (float)deltaTime);
        }
    }

    private void InterpolateAllQuads(float interpolationSpeed, Vector3 zero, int endDistance, float deltaTime)
    {
        foreach (var quadBatch in _quadBatches)
        {
            var index = 0;
            foreach (var quad in quadBatch.Quads)
            {
                var squareDistanceToCube = SPNMath.SquareDistance(quad.Position, zero);
                var isOutsideCube = squareDistanceToCube > Math.Pow(endDistance, 2);
                if (isOutsideCube)
                {
                    quad.Position = SPNMath.Lerp(quad.Position, zero, interpolationSpeed * _random.Next(0, 2) * (float)deltaTime);
                    quad.Rotation += new Vector3(0.55f, 0, 0) * deltaTime;
                    quadBatch.InvalidatedQuads.Add((index, quad));

                    index++;
                }
            }
        }
    }

    //private void InterpolateRandomQuads(int randomQuadCount, float interpolationSpeed, Vector3 cubePosition, int cubeSize)
    //{
    //    foreach (var quadBatch in _quadBatches)
    //    {
    //        var randomQuads = quadBatch.Quads.TakeRandomWithIndex(randomQuadCount);
    //        foreach (var quad in randomQuads)
    //        {
    //            var squareDistanceToCube = SPNMath.SquareDistance(quad.quad.Position, cubePosition);
    //            var isOutsideCube = squareDistanceToCube > Math.Pow(cubeSize, 2);
    //            if (isOutsideCube)
    //            {
    //                quad.quad.Position = SPNMath.Lerp(quad.quad.Position, cubePosition, interpolationSpeed);
    //                quadBatch.InvalidatedQuads.Add((quad.index, quad.quad));
    //            }
    //        }
    //    }
    //}

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
