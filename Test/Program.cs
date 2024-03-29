using GameEngine.Configs;
using GameEngine.Logging;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Test.Config;

namespace Test;

public class Program
{
    public static TestGame? Game { get; set; }

    private static void Main()
    {
        var config = ConfigManager.LoadConfig<GameConfig>(GameConstants.GameConfigPath);
        if (config is null)
        {
            Logger.GameLogger.Error("Failed to load game config.");
            return;
        }

        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new OpenTK.Mathematics.Vector2i(config.Width, config.Height),
            Title = config.Title,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible,
            WindowBorder = WindowBorder.Fixed,
            WindowState = config.Fullscreen ? WindowState.Fullscreen : WindowState.Normal,
            StartVisible = false,
        };

        Game = new TestGame(GameConstants.Title, nativeWindowSettings: nativeWindowSettings);
    }
}
