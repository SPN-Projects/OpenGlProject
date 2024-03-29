using System.Numerics;
using GameEngine;

namespace Test;
public class GameConstants
{
    // GameConfig
    public static readonly string GameConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EngineConstants.Organization, "GameConfig.yaml");
    public const string Title = "Test Game";
    public static readonly Vector2 DefaultWindowSize = new(1280, 720);
    public const bool FullScreen = false;
    public const bool VSync = true;
}
