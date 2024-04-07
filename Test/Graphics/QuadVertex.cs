using OpenTK.Mathematics;

namespace Test.Graphics;
internal struct QuadVertex
{
    public Vector3 Position { get; set; }
    public Vector2 TexCoord { get; set; }
}

internal struct QuadInstancedData
{
    public Vector4 Color { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public float TextureIndex { get; set; }
}

internal struct QuadVertexColor
{
    public Vector4 Color { get; set; }
}
