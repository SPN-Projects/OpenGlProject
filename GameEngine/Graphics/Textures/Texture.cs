namespace GameEngine.Graphics.Textures;
public abstract class Texture : IDisposable, IEquatable<Texture>
{
    public int Handle { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public float TilingFactor { get; protected set; } = 1.0f;

    public byte[]? Data { get; protected set; }

    public abstract void Bind(int slot = 0);
    public abstract void Dispose();


    public abstract override bool Equals(object? obj);
    public abstract bool Equals(Texture? other);
    public abstract override int GetHashCode();

}
