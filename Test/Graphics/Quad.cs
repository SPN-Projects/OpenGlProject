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

    private Vector3 _rotation;
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            IsValid = false;
        }
    }

    private Vector2 _scale;
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
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
        Scale = size;
        Color = color;
        _originalColor = color;

        IsVisible = true;

        UpdateModelMatrix();
        Validate();
    }

    public void Validate()
        => IsValid = true;

    private void UpdateModelMatrix()
        => ModelMatrix = Matrix4.CreateScale(new Vector3(Scale.X, Scale.Y, 0)) * Matrix4.CreateTranslation(-Position);

    internal void Recalculate()
        => UpdateModelMatrix();
}

public class QuadBatch : IDisposable
{
    public Shader Shader { get; set; }
    public Texture? Texture { get; set; }
    public readonly List<Quad> Quads;
    public readonly int QuadsLimit;

    public List<(int, Quad)> InvalidatedQuads { get; set; }

    private bool _disposedValue;

    private VertexArrayObject? _vao;
    private VertexBufferObject? _vbo;
    private VertexBufferObject? _instancedVbo;
    private readonly VertexBufferObject? _instanceColorVbo;
    private ElementBufferObject? _ebo;

    private static readonly Vector4[] _quadVertexPositions =
    [
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f, 0.5f, 0.0f, 1.0f),
        new(-0.5f, 0.5f, 0.0f, 1.0f)
    ];

    private static readonly Vector2[] _quadTextureCoordinates =
    [
        new(0.0f, 0.0f),
        new(1.0f, 0.0f),
        new(1.0f, 1.0f),
        new(0.0f, 1.0f)
    ];

    public QuadBatch(int quadsLimit, Shader quadShader, Texture? texture = null)
    {
        QuadsLimit = quadsLimit;
        Texture = texture;
        Shader = quadShader;
        Quads = [];
        InvalidatedQuads = [];

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
            new(ShaderDataType.Float2, "aTexCoord")
        ]);
        _vbo = VertexBufferObject.FromBufferLayout(quadLayout, BufferUsageHint.StaticDraw, QuadsLimit * Quad.VerticesCount);
        _vao.AddVertexBufferObject(_vbo);

        var instancedLayout = new BufferLayout(
                   [
            new(ShaderDataType.Float4, "aColor", false, 1),
            new(ShaderDataType.Float3, "aPosition", false, 1),
            new(ShaderDataType.Float3, "aRotation", false, 1),
            new(ShaderDataType.Float2, "aSize", false, 1),
        ]);
        _instancedVbo = VertexBufferObject.FromBufferLayout(instancedLayout, BufferUsageHint.StaticDraw, QuadsLimit);
        _vao.AddVertexBufferObject(_instancedVbo);

        //var instanceColorLayout = new BufferLayout(
        //           [
        //    new(ShaderDataType.Float4, "aColor", false, 1)
        //]);
        //_instanceColorVbo = VertexBufferObject.FromBufferLayout(instanceColorLayout, BufferUsageHint.StaticDraw, QuadsLimit);
        //_vao.AddVertexBufferObject(_instanceColorVbo);

        // Element Buffer Object
        var offset = 0;
        var quadIndicesCount = 6;
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
                TexCoord = _quadTextureCoordinates[i]
            };

            quadVertices[i] = quadVertex;
        }

        _vbo.SetData(quadVertices);
    }

    public void AddQuad(Quad quad)
    {
        if (Quads.Count < QuadsLimit)
        {
            Quads.Add(quad);
        }
        else
        {
            Logger.EngineLogger.Warning("Exceeded the maximum number of quads.");
            throw new InvalidOperationException("Exceeded the maximum number of quads.");
        }
    }

    public void RecalculateQuadsModels((int, Quad)[] invalidatedQuads)
    {
        if (invalidatedQuads.Length == QuadsLimit)
        {
            RecalculateAll();
        }
        else
        {
            foreach (var (index, quad) in invalidatedQuads)
            {
                var instancedData = new QuadInstancedData
                {
                    Color = quad.Color,
                    Position = quad.Position,
                    Rotation = quad.Rotation,
                    Size = quad.Scale,
                };

                _instancedVbo!.SetData(instancedData, index * Marshal.SizeOf(typeof(QuadInstancedData)));
            }
        }
    }

    public void Draw(Camera? camera, (int, Quad)[] invalidatedQuads)
    {
        camera ??= new IdentityCamera();

        Shader!.Bind();

        Shader.SetUniform("uViewProjectionMatrix", camera.ViewProjectionMatrix);

        if (invalidatedQuads.Length > 0)
        {
            RecalculateQuadsModels(invalidatedQuads);
        }

        Texture?.Bind();
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
        var instancedData = new ConcurrentBag<QuadInstancedData>();
        _ = Parallel.ForEach(Quads, (quad) =>
        {
            instancedData.Add(new QuadInstancedData
            {
                Position = quad.Position,
                Size = quad.Scale,
                Rotation = quad.Rotation,
                Color = quad.Color
            });
        });

        _instancedVbo!.SetData(instancedData.ToArray());
    }
}
