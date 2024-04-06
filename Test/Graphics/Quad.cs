using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using GameEngine.Enums;
using GameEngine.Graphics.Buffers;
using GameEngine.Graphics.Cameras;
using GameEngine.Graphics.Shaders;
using GameEngine.Graphics.Textures;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Test.Graphics;
public class Quad
{
    public const int VerticesCount = 4;
    public bool IsValid { get; private set; }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            Color = value ? _originalColor : new Vector4(0, 0, 0, 0);
        }
    }

    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            IsValid = false;
        }
    }

    private Vector2 _size;
    public Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            IsValid = false;
        }
    }

    private Vector4 _color;
    private Vector4 _originalColor;
    public Vector4 Color
    {
        get => _color;
        set
        {
            _color = value;
            IsValid = false;
        }
    }

    public Matrix4 ModelMatrix { get; private set; }

    public Quad(Vector3 position, Vector2 size, Vector4 color)
    {
        Position = position;
        Size = size;
        Color = color;
        _originalColor = color;

        IsVisible = true;

        UpdateModelMatrix();
        Validate();
    }

    public void Validate()
        => IsValid = true;

    private void UpdateModelMatrix()
        => ModelMatrix = Matrix4.CreateScale(new Vector3(Size.X, Size.Y, 0)) * Matrix4.CreateTranslation(-Position);

    internal void Recalculate()
        => UpdateModelMatrix();
}

public class QuadBatch : IDisposable
{
    public Shader Shader { get; set; }
    public Texture? Texture { get; set; }
    public readonly List<Quad> Quads;

    public ConcurrentDictionary<int, Quad> InvalidatedQuads { get; private set; }

    private bool _disposedValue;
    private readonly int _quadsLimit;

    private VertexArrayObject? _vao;
    private VertexBufferObject? _vbo;
    private VertexBufferObject? _instancedVbo;
    private VertexBufferObject? _instanceColorVbo;
    private ElementBufferObject? _ebo;

    private static readonly Vector4[] _quadVertexPositions =
    [
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, 0.5f, 0.0f, 1.0f),
        new(-0.5f, 0.5f, 0.0f, 1.0f)
    ];

    public QuadBatch(int quadsLimit, Shader quadShader, Texture? texture = null)
    {
        _quadsLimit = quadsLimit;
        Texture = texture;
        Shader = quadShader;
        Quads = [];
        InvalidatedQuads = new ConcurrentDictionary<int, Quad>();

        InitBuffers();
    }

    private void InitBuffers()
    {
        // create vertex array object
        _vao = new VertexArrayObject();

        // Vertex Buffer Object + Buffer Layout
        var quadLayout = new BufferLayout(
        [
            new(ShaderDataType.Float3, "aPosition"),
        ]);
        _vbo = VertexBufferObject.FromBufferLayout(quadLayout, BufferUsageHint.StaticDraw, _quadsLimit * Quad.VerticesCount);
        _vao.AddVertexBufferObject(_vbo);

        var instancedLayout = new BufferLayout(
                   [
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1),
            new(ShaderDataType.Float4, "aModel", false, 1)
        ]);
        _instancedVbo = VertexBufferObject.FromBufferLayout(instancedLayout, BufferUsageHint.StaticDraw, _quadsLimit);
        _vao.AddVertexBufferObject(_instancedVbo);

        var instanceColorLayout = new BufferLayout(
                   [
            new(ShaderDataType.Float4, "aColor", false, 1)
        ]);
        _instanceColorVbo = VertexBufferObject.FromBufferLayout(instanceColorLayout, BufferUsageHint.StaticDraw, _quadsLimit);
        _vao.AddVertexBufferObject(_instanceColorVbo);

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

        var quadVertices = new QuadVertex[4];
        for (var i = 0; i < _quadVertexPositions.Length; i++)
        {
            var quadVertex = new QuadVertex
            {
                Position = _quadVertexPositions[i].Xyz,
            };

            quadVertices[i] = quadVertex;
        }

        _vbo.SetData(quadVertices);
    }

    public void AddQuad(Quad quad)
    {
        if (Quads.Count < _quadsLimit)
        {
            Quads.Add(quad);
        }
        else
        {
            Logger.EngineLogger.Warning("Exceeded the maximum number of quads.");
            throw new InvalidOperationException("Exceeded the maximum number of quads.");
        }
    }

    public void RecalculateQuadsModels(KeyValuePair<int, Quad>[] invalidatedQuads)
    {
        foreach (var (index, quad) in invalidatedQuads)
        {
            _instanceColorVbo!.SetData(quad.Color, index * Marshal.SizeOf(typeof(Vector4)));
            _instancedVbo!.SetData(quad.ModelMatrix, index * Marshal.SizeOf(typeof(Matrix4)));
        }
    }

    public void Draw(Camera? camera, KeyValuePair<int, Quad>[] invalidatedQuads)
    {
        camera ??= new IdentityCamera();

        Shader!.Bind();

        Shader.SetUniform("uViewProjectionMatrix", camera.ViewProjectionMatrix);

        if (invalidatedQuads.Length > 0)
        {
            RecalculateQuadsModels(invalidatedQuads);
        }

        _vao!.Bind();
        GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0, Quads.Count);
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

    internal void RecalculateAll()
    {
        _instanceColorVbo!.SetData(Quads.Select(q => q.Color).ToArray());
        _instancedVbo!.SetData(Quads.Select(q => q.ModelMatrix).ToArray());
    }
}
