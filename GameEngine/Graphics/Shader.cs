using GameEngine.Logging;
using GameEngine.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine.Graphics;
public partial class Shader : IDisposable
{
    public int Handle { get; }

    public Dictionary<int, int> Uniforms { get; }

    public static Shader Default
        => FromName(EngineConstants.DefaultShaderName);

    private readonly Dictionary<ShaderType, string> _shaderSources;
    private string? _shaderName;
    private bool _disposedValue;

    /// <summary>
    /// Creates a new Shader from the Name of the Shader, within the default Shader Path (e.g. "Default")
    /// -> Default.vert, Default.frag, Default.geom, ...
    /// </summary>
    /// <param name="name">The name of the Shader</param>
    /// <returns>The new Shader</returns>
    /// <exception cref="Exception">Thrown when the Shader with the given name is not found, or an error occurred whilst compiling the Shader</exception>
    public static Shader FromName(string name)
    {
        PathChecker.EnsurePath(EngineConstants.DefaultShaderPath);
        var shaderFiles = Directory.GetFiles(EngineConstants.DefaultShaderPath, name + "*");

        if (shaderFiles.Length == 0)
        {
            Logger.EngineLogger.Error($"Shader with name {name} not found");
            throw new Exception($"Shader with name {name} not found");
        }

        var shader = new Shader
        {
            _shaderName = name
        };

        try
        {
            foreach (var shaderFile in shaderFiles)
            {
                shader.ParseFromFile(shaderFile);
            }

            shader.CompileShaderSources();
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error(e.Message);
            throw new Exception(e.Message);
        }

        Logger.EngineLogger.Trace($"Shader with name {name} loaded successfully");
        return shader;
    }

    /// <summary>
    /// Bind the Shader for use (needed before rendering to ensure the Shader is used for the correct buffers)
    /// </summary>
    public void Use()
        => GL.UseProgram(Handle);

    #region SetUniforms
    public void SetUniform(string name, int data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform1(location, data);
    }

    public void SetUniform(string name, float data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform1(location, data);
    }

    public void SetUniform(string name, Vector2 data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform2(location, data);
    }

    public void SetUniform(string name, Vector3 data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform3(location, data);
    }

    public void SetUniform(string name, Vector4 data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform4(location, data);
    }

    public void SetUniform(string name, Matrix4 data)
    {
        var location = GetUniformLocation(name);
        GL.UniformMatrix4(location, false, ref data);
    }

    public void SetUniform(string name, bool data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform1(location, data ? 1 : 0);
    }

    public void SetUniform(string name, int[] data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform1(location, data.Length, data);
    }

    public void SetUniform(string name, float[] data)
    {
        var location = GetUniformLocation(name);
        GL.Uniform1(location, data.Length, data);
    }

    #endregion

    private Shader()
    {
        Handle = GL.CreateProgram();
        Uniforms = [];
        _shaderSources = [];
    }

    /// <summary>
    /// Parses one Shader Source from the given file path
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="Exception">Thrown when an error occurred whilst parsing the Shader Source</exception>
    private void ParseFromFile(string path)
    {
        try
        {
            PathChecker.EnsureFile(path);
            ParseShaderSourceFromFile(path);
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error(e.Message);
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Parses the Shader Sources from the given file path
    /// </summary>
    /// <param name="filePath"></param>
    private void ParseShaderSourceFromFile(string filePath)
    {
        var shaderSource = File.ReadAllText(filePath);
        var shaderType = GetShaderTypeFromPath(filePath);

        _shaderSources.Add(shaderType, shaderSource);
    }

    /// <summary>
    /// Gets the Shader Type from the given Shader File Type
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="Exception">Thrown when the Shader Type is invalid or not found</exception>
    private static ShaderType GetShaderTypeFromPath(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        return extension switch
        {
            EngineConstants.VertexShaderExtension => ShaderType.VertexShader,
            EngineConstants.FragmentShaderExtension => ShaderType.FragmentShader,
            EngineConstants.GeometryShaderExtension => ShaderType.GeometryShader,
            _ => throw new Exception("Invalid Shader Type")
        };
    }

    /// <summary>
    /// Compiles the Shader Sources and links them to the Shader Program, 
    /// then deletes the Shader Handles (Handles are no longer needed, because the Shader is already linked)
    /// </summary>
    /// <exception cref="Exception">Thrown when an error occurred whilst Linking and Compiling the Shader</exception>
    private void CompileShaderSources()
    {
        var shaderHandles = new List<int>();
        try
        {
            foreach (var (shaderType, shaderSource) in _shaderSources)
            {
                var shaderHandle = GL.CreateShader(shaderType);
                shaderHandles.Add(shaderHandle);

                GL.ShaderSource(shaderHandle, shaderSource);
                GL.CompileShader(shaderHandle);

                GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var result);
                if (result != (int)All.True)
                {
                    var infoLog = GL.GetShaderInfoLog(shaderHandle);

                    Logger.EngineLogger.Error($"Error occurred whilst compiling Shader ({_shaderName}) [{shaderType}]: {infoLog}");
                    throw new Exception($"Error occurred whilst compiling Shader ({_shaderName}) [{shaderType}]: {infoLog}");
                }

                GL.AttachShader(Handle, shaderHandle);
            }

            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(Handle);

                Logger.EngineLogger.Error($"Error occurred whilst linking Shader: {infoLog}");
                throw new Exception($"Error occurred whilst linking Shader: {infoLog}");
            }

            foreach (var shaderHandle in shaderHandles)
            {
                GL.DetachShader(Handle, shaderHandle);
                GL.DeleteShader(shaderHandle);
            }
        }
        catch (Exception e)
        {
            GL.DeleteProgram(Handle);

            Logger.EngineLogger.Error(e.Message);
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Gets the Uniform Location for the given Uniform Name in the Shader (caches the Uniform Location for future use)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private int GetUniformLocation(string name)
    {
        if (Uniforms.ContainsKey(name.GetHashCode()))
        {
            return Uniforms[name.GetHashCode()];
        }

        var location = GL.GetUniformLocation(Handle, name);
        Uniforms.Add(name.GetHashCode(), location);

        return location;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Logger.EngineLogger.Info("Unloading Shader");
                GL.DeleteProgram(Handle);
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
