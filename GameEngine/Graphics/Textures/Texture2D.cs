using System.Drawing;
using System.Runtime.Versioning;
using GameEngine.Logging;
using GameEngine.Utils;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameEngine.Graphics.Textures;
public class Texture2D : Texture
{
    private readonly PixelFormat _pixelFormat;
    private readonly SizedInternalFormat _internalFormat;

    public static Texture2D White
    {
        get
        {
            var texture = new Texture2D(1, 1);
            texture.SetData([255, 255, 255, 255]);
            return texture;
        }
    }

    public Texture2D(int width, int height)
    {
        Width = width;
        Height = height;

        _pixelFormat = PixelFormat.Rgba;
        _internalFormat = SizedInternalFormat.Rgba8;

        GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
        Handle = handle;

        GL.TextureStorage2D(Handle, 1, SizedInternalFormat.Rgba8, width, height);

        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    }

    public Texture2D(string path)
    {
        _internalFormat = SizedInternalFormat.Rgba8;
        _pixelFormat = PixelFormat.Rgba;

        var image = ImageManager.LoadImage(path, ColorComponents.RedGreenBlueAlpha, true);

        Width = image.Width;
        Height = image.Height;

        GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
        Handle = handle;

        GL.TextureStorage2D(Handle, 1, _internalFormat, Width, Height);

        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    // attribute to call only on windows machines
    [SupportedOSPlatform("windows")]
    public Texture2D(Bitmap bitmap)
    {
        Width = bitmap.Width;
        Height = bitmap.Height;

        _pixelFormat = PixelFormat.Bgra;
        _internalFormat = SizedInternalFormat.Rgba8;

        GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
        Handle = handle;

        GL.TextureStorage2D(Handle, 1, _internalFormat, Width, Height);

        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, _pixelFormat, PixelType.UnsignedByte, data.Scan0);
        bitmap.UnlockBits(data);
    }

    public override void Bind(int slot = 0)
        => GL.BindTextureUnit(slot, Handle);

    public void SetData(byte[] data, bool generateMipmap = true)
    {
        Data = data;

        var bytesPerPixel = _pixelFormat switch
        {
            PixelFormat.Rgba => 4,
            PixelFormat.Rgb => 3,
            _ => throw new NotSupportedException("The pixel format is not supported.")
        };

        if (data.Length != Width * Height * bytesPerPixel)
        {
            Logger.EngineLogger.Error("Data length does not match the texture size.");
            throw new ArgumentException("Data length does not match the texture size.");
        }

        GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, _pixelFormat, PixelType.UnsignedByte, data);

        if (generateMipmap)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.GenerateTextureMipmap(Handle);
        }
    }

    public override void Dispose()
    {
        GL.DeleteTexture(Handle);
        GC.SuppressFinalize(this);
    }

    public override bool Equals(object? obj)
        => obj is not null and Texture2D texture2D && Equals(texture2D);

    public override bool Equals(Texture? other)
        => other is Texture2D texture2D && Handle == texture2D.Handle;

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Handle);
        return hash.ToHashCode();
    }
}
