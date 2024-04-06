using GameEngine.Enums;

namespace GameEngine.Graphics.Buffers;

public class BufferElement(ShaderDataType type, string name, bool normalized = false, int divisor = 0)
{
    public string Name { get; } = name;
    public ShaderDataType Type { get; } = type;

    public int ComponentCount { get; } = type.Count;
    public int Size { get; } = type.Size;
    public int Offset { get; internal set; } = 0;
    public bool Normalized { get; } = normalized;

    public int Divisor { get; internal set; } = divisor;
}