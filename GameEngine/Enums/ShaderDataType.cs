using OpenTK.Graphics.OpenGL4;

namespace GameEngine.Enums;
public class ShaderDataType(int id, string name, uint size, uint count) : BaseEnum<ShaderDataType>(id, name)
{
    public static readonly ShaderDataType Float = new(0, "Float", 4, 1);
    public static readonly ShaderDataType Float2 = new(1, "Float2", 4 * 2, 2);
    public static readonly ShaderDataType Float3 = new(2, "Float3", 4 * 3, 3);
    public static readonly ShaderDataType Float4 = new(3, "Float4", 4 * 4, 4);
    public static readonly ShaderDataType Mat3 = new(4, "Mat3", 4 * 3 * 3, 3 * 3);
    public static readonly ShaderDataType Mat4 = new(5, "Mat4", 4 * 4 * 4, 4 * 4);
    public static readonly ShaderDataType Int = new(6, "Int", 4, 1);
    public static readonly ShaderDataType Int2 = new(7, "Int2", 4 * 2, 2);
    public static readonly ShaderDataType Int3 = new(8, "Int3", 4 * 3, 3);
    public static readonly ShaderDataType Int4 = new(9, "Int4", 4 * 4, 4);
    public static readonly ShaderDataType Bool = new(10, "Bool", 1, 1);

    public int Size { get; init; } = (int)size;
    public int Count { get; init; } = (int)count;

    public static implicit operator VertexAttribPointerType(ShaderDataType type)
        => type.Id switch
        {
            0 => VertexAttribPointerType.Float,
            1 => VertexAttribPointerType.Float,
            2 => VertexAttribPointerType.Float,
            3 => VertexAttribPointerType.Float,
            4 => VertexAttribPointerType.Float,
            5 => VertexAttribPointerType.Float,
            6 => VertexAttribPointerType.Int,
            7 => VertexAttribPointerType.Int,
            8 => VertexAttribPointerType.Int,
            9 => VertexAttribPointerType.Int,
            10 => VertexAttribPointerType.Int,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
}
