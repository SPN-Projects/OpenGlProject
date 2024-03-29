using System.Numerics;

namespace GameEngine;
public static class EngineConstants
{
    // Logger
    public static readonly ConsoleColor ConsoleLoggerEngineProjectColor = ConsoleColor.Magenta;
    public static readonly ConsoleColor ConsoleLoggerGameProjectColor = ConsoleColor.Cyan;

    public static readonly ConsoleColor ConsoleLoggerTraceColor = ConsoleColor.Gray;
    public static readonly ConsoleColor ConsoleLoggerDebugColor = ConsoleColor.Blue;
    public static readonly ConsoleColor ConsoleLoggerInfoColor = ConsoleColor.Green;
    public static readonly ConsoleColor ConsoleLoggerWarningColor = ConsoleColor.Yellow;
    public static readonly ConsoleColor ConsoleLoggerErrorColor = ConsoleColor.Red;
    public static readonly ConsoleColor ConsoleLoggerCriticalColor = ConsoleColor.DarkRed;

    // Engine
    public static Vector2 DefaultWindowSize = new(1280, 720);

    // Shader
    public const string VertexShaderExtension = ".vert";
    public const string FragmentShaderExtension = ".frag";
    public const string GeometryShaderExtension = ".geom";

    public const string DefaultShaderName = "Default";
    public const string DefaultResourcePath = "Resources/";
    public const string DefaultShaderPath = "Resources/Shaders/";

    public const string DefaultVertexShaderPath = "Resources/Shaders/Default.vert";
    public const string DefaultFragmentShaderPath = "Resources/Shaders/Default.frag";
}
