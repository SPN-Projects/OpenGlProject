using OpenTK.Mathematics;

namespace Test.Graphics;
internal struct QuadVertex
{
    public Vector3 Position { get; set; }
}

internal struct QuadVertexModel
{
    public Matrix4 Model { get; set; }
}

internal struct QuadVertexColor
{
    public Vector4 Color { get; set; }
}
