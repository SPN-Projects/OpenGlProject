using GameEngine.Configs;
using GameEngine.Logging;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Test.Config;

namespace Test;

public class Program
{
    public static TestGame? Game { get; private set; }
    public static GameConfig? GameConfig { get; private set; }

    private static void Main()
    {
        GameConfig = ConfigManager.LoadConfig<GameConfig>(GameConstants.GameConfigPath);
        if (GameConfig is null)
        {
            Logger.GameLogger.Error("Failed to load game config.");
            return;
        }

        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new OpenTK.Mathematics.Vector2i(GameConfig.Width, GameConfig.Height),
            Title = GameConfig.Title,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible,
            WindowBorder = WindowBorder.Fixed,
            WindowState = WindowState.Normal,
            StartVisible = false,
            Vsync = GameConfig.VSync ? VSyncMode.On : VSyncMode.Off
        };

        Game = new TestGame(GameConstants.Title, nativeWindowSettings: nativeWindowSettings);
    }
}
