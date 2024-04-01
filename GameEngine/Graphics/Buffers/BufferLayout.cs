using System.Collections;

namespace GameEngine.Graphics.Buffers;

public class BufferLayout : IEnumerable<BufferElement>
{
    public List<BufferElement> Elements { get; }
    public int Stride { get; private set; }

    public BufferLayout(params BufferElement[] elements)
    {
        Elements = new List<BufferElement>(elements);
        Stride = 0;
        CalculateOffsetAndStride();
    }

    private void CalculateOffsetAndStride()
    {
        var offset = 0;

        foreach (var element in Elements)
        {
            element.Offset = offset;
            offset += element.Size;
            Stride += element.Size;
        }
    }

    public IEnumerator<BufferElement> GetEnumerator()
        => Elements.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return GetEnumerator();
    }
}