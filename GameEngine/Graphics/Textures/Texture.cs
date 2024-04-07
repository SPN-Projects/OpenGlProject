using GameEngine.Logging;
using GameEngine.Utils;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GameEngine.Graphics.Textures;
public class Texture : IDisposable
{
    private bool _disposedValue;
    public int Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public byte[]? Data { get; private set; }

    public static Texture Default
        => FromFile(EngineConstants.DefaultTexturePath);

    public Texture(int width, int height)
    {
        Width = width;
        Height = height;

        Handle = GL.GenTexture();

        GL.TextureStorage2D(Handle, 1, SizedInternalFormat.Rgba8, width, height);

        GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    }

    public static Texture FromFile(string path)
    {
        try
        {
            PathChecker.EnsureFile(path);

            // Flip the image vertically, because OpenGL draws the texture from left to right, bottom to top
            // and stbi loads the image from top to bottom
            StbImage.stbi_set_flip_vertically_on_load(1);

            var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            if (image == null)
            {
                Logger.EngineLogger.Error($"Failed to load image from {path}");
                throw new Exception("Failed to load image");
            }

            var newTexture = new Texture(image.Width, image.Height);
            newTexture.SetData(image.Data);

            Logger.EngineLogger.Trace($"Texture loaded from {path}");
            return newTexture;
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error(e.Message);
            throw new Exception(e.Message);
        }
    }

    public void Bind()
        => GL.BindTexture(TextureTarget.Texture2D, Handle);

    public void SetData(byte[] data, bool createMipmap = true)
    {
        Data = data;

        Bind();
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

        if (createMipmap)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Logger.EngineLogger.Trace($"Disposing texture {Handle}");
                GL.DeleteTexture(Handle);
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
