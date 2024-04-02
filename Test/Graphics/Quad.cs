using System.Runtime.InteropServices;
using GameEngine.Enums;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Shaders;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Test.Graphics;
public class Quad
{
    public Vector3 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector4 Color { get; set; }

    public Quad(Vector3 position, Vector2 size, Vector4 color)
    {
        Position = position;
        Size = size;
        Color = color;
    }
}

public class QuadBatch : IDisposable
{

    public Shader Shader { get; set; }
    private readonly List<Quad> _quads;

    private bool _disposedValue;
    private readonly int _quadsLimit;

    private VertexArrayObject? _vao;
    private VertexBufferObject? _vbo;
    private VertexBufferObject? _instancedVbo;
    private VertexBufferObject? _instanceColorVbo;
    private ElementBufferObject? _ebo;

    private List<QuadVertex> _quadVertexBufferList;
    private static readonly Vector4[] _quadVertexPositions =
    [
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, 0.5f, 0.0f, 1.0f),
        new(-0.5f, 0.5f, 0.0f, 1.0f)
    ];

    public QuadBatch(int quadsLimit, Shader quadShader)
    {
        _quadsLimit = quadsLimit;
        _quadVertexBufferList = [];
        Shader = quadShader;
        _quads = [];

        InitBuffers();
    }

    private void InitBuffers()
    {
        // create vertex array object
        _vao = new VertexArrayObject();

        // Vertex Buffer Object + Buffer Layout
        _vbo = new VertexBufferObject(_quadsLimit + Marshal.SizeOf(typeof(QuadVertex)), BufferUsageHint.StaticDraw);
        var quadLayout = new BufferLayout(
        [
            new(ShaderDataType.Float3, "aPosition"),
        ]);

        _vbo.SetBufferLayout(quadLayout);
        _vao.AddVertexBufferObject(_vbo);

        _instancedVbo = new VertexBufferObject(_quadsLimit * Marshal.SizeOf(typeof(QuadVertexModel)), BufferUsageHint.StaticDraw);
        var instancedLayout = new BufferLayout(
                   [
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1)
        ]);

        _instancedVbo.SetBufferLayout(instancedLayout);
        _vao.AddVertexBufferObject(_instancedVbo, 1);

        _instanceColorVbo = new VertexBufferObject(_quadsLimit * Marshal.SizeOf(typeof(QuadVertexColor)), BufferUsageHint.StaticDraw);
        var instanceColorLayout = new BufferLayout(
                   [
            new(ShaderDataType.Float4, "aColor", false, 1)
        ]);

        _instanceColorVbo.SetBufferLayout(instanceColorLayout);
        _vao.AddVertexBufferObject(_instanceColorVbo, 5);

        // Element Buffer Object
        var offset = 0;
        var quadIndicesCount = _quadsLimit * 6;
        var quadIndices = new uint[quadIndicesCount];
        for (var i = 0; i < quadIndicesCount; i += 6)
        {
            quadIndices[i + 0] = (uint)(offset + 0);
            quadIndices[i + 1] = (uint)(offset + 1);
            quadIndices[i + 2] = (uint)(offset + 2);
            quadIndices[i + 3] = (uint)(offset + 2);
            quadIndices[i + 4] = (uint)(offset + 3);
            quadIndices[i + 5] = (uint)(offset + 0);

            offset += 4;
        }

        _ebo = new ElementBufferObject(quadIndices, quadIndicesCount, BufferUsageHint.StaticDraw);
        _vao.SetElementBufferObject(_ebo);

        var maxVertices = _quadsLimit * 4;
        _quadVertexBufferList = new(maxVertices);

        var quadVertices = new QuadVertex[4];
        for (var i = 0; i < _quadVertexPositions.Length; i++)
        {
            var quadVertex = new QuadVertex
            {
                Position = _quadVertexPositions[i].Xyz,
            };

            quadVertices[i] = quadVertex;
        }

        _vbo.SetData(quadVertices.ToList());
    }

    public void AddQuad(Quad quad)
    {
        if (_quads.Count < _quadsLimit)
        {
            _quads.Add(quad);
        }
        else
        {
            Logger.EngineLogger.Warning("Exceeded the maximum number of quads.");
            throw new InvalidOperationException("Exceeded the maximum number of quads.");
        }
    }

    public void RecalculateQuadsModels()
    {
        var modelList = new List<Matrix4>();
        var colorList = new List<Vector4>();
        foreach (var quad in _quads)
        {
            var color = quad.Color;
            var model = Matrix4.CreateScale(new Vector3(quad.Size.X, quad.Size.Y, 0)) * Matrix4.CreateTranslation(-quad.Position);

            colorList.Add(color);
            modelList.Add(model);
        }

        _instanceColorVbo!.SetData(colorList);
        _instancedVbo!.SetData(modelList);
    }

    public void Draw(Camera? camera)
    {
        camera ??= new IdentityCamera();

        Shader!.Bind();

        Shader.SetUniform("uViewProjectionMatrix", camera.ViewProjectionMatrix);

        _vao!.Bind();
        GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0, _quads.Count);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _vao?.Dispose();
                _instancedVbo?.Dispose();
                _vbo?.Dispose();
                _ebo?.Dispose();

                Shader?.Dispose();
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
