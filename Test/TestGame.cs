using System.Runtime.InteropServices;
using GameEngine;
using GameEngine.Enums;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Rendering;
using GameEngine.Graphics.Shaders;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Test.Graphics;

namespace Test;
public class TestGame : Game, IDisposable
{
    private Shader? _shader;
    private readonly int _vertexBufferObject;
    private readonly int _vertexArrayObject;
    private readonly int _elementBufferObject;

    private readonly Random _random = new();

    private readonly int _maxElementCount = 1000;
    private VertexArrayObject _vao;
    private VertexBufferObject _vbo;
    private ElementBufferObject _ebo;
    private List<QuadVertex> _quadVertexBufferList;
    private Vector4[] _quadVertexPositions;

    private Vector3[] _randomColors;

    private Camera _camera;

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

        // create vertex array object
        _vao = new VertexArrayObject();

        // Vertex Buffer Object + Buffer Layout
        _vbo = new VertexBufferObject(_maxElementCount + Marshal.SizeOf(typeof(QuadVertex)), BufferUsageHint.StaticDraw);
        var quadLayout = new BufferLayout(
        [
            new(ShaderDataType.Float3, "aPosition"),
            new(ShaderDataType.Float3, "aColor")
        ]);

        _vbo.SetBufferLayout(quadLayout);
        _vao.AddVertexBufferObject(_vbo);

        // Element Buffer Object
        var offset = 0;
        var quadIndicesCount = _maxElementCount * 6;
        var quadIndices = new uint[quadIndicesCount];
        for (var i = 0; i < quadIndicesCount; i += 6)
        {
            quadIndices[i + 0] = (uint)(offset + 0);
            quadIndices[i + 1] = (uint)(offset + 1);
            quadIndices[i + 2] = (uint)(offset + 2);
            quadIndices[i + 3] = (uint)(offset + 2);
            quadIndices[i + 4] = (uint)(offset + 3);
            quadIndices[i + 5] = (uint)(offset + 0);

            offset += 6;
        }

        _ebo = new ElementBufferObject(quadIndices, quadIndicesCount, BufferUsageHint.StaticDraw);
        _vao.SetElementBufferObject(_ebo);

        var maxVertices = _maxElementCount * 4;
        _quadVertexBufferList = new(maxVertices);
        _quadVertexPositions =
        [
            new(-0.5f, -0.5f, 0.0f, 1.0f),
            new(0.5f, -0.5f, 0.0f, 1.0f),
            new(0.5f, 0.5f, 0.0f, 1.0f),
            new(-0.5f, 0.5f, 0.0f, 1.0f)
        ];

        // create shader
        _shader = Shader.Default;

        _randomColors = [
            new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle()),
            new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle()),
            new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle()),
            new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle())
        ];

        _camera = new Camera(Matrix4.CreateTranslation(new(1280 / 2, 720 / 2, 0)) * Matrix4.CreateOrthographicOffCenter(0, 1280, 720, 0, -1.0f, 1.0f));
        //_camera = new Camera();
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

        Renderer.BeginScene(_camera);
        _quadVertexBufferList.Clear();
        for (var i = 0; i < _quadVertexPositions.Length; i++)
        {
            var quad = new QuadVertex
            {
                Position = (Matrix4.CreateScale(100, 100, 0) * _quadVertexPositions[i]).Xyz,
                Color = _randomColors[i]
            };

            _quadVertexBufferList.Add(quad);
        }

        _vbo.SetData(_quadVertexBufferList);
        _shader!.Bind();
        _shader.SetUniform("uViewProjectionMatrix", _camera.ViewProjectionMatrix);

        _vao.Bind();
        var drawCount = _vao.ElementBufferObject!.Count;
        GL.DrawElements(PrimitiveType.Triangles, drawCount, DrawElementsType.UnsignedInt, 0);

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
