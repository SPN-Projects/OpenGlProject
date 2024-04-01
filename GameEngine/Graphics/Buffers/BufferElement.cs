using GameEngine.Enums;

namespace GameEngine.Graphics.Buffers;

public class BufferElement
{
    public string Name { get; }
    public ShaderDataType Type { get; }

    public int ComponentCount { get; }
    public int Size { get; }
    public int Offset { get; internal set; }

    public bool Normalized { get; }

    public BufferElement(ShaderDataType type, string name, bool normalized = false)
    {
        Name = name;
        Type = type;
        ComponentCount = type.Count;
        Size = type.Size;
        Offset = 0;
        Normalized = normalized;
    }
}