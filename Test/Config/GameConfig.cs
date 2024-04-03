namespace Test.Config;
public class GameConfig
{
    public string Title { get; set; } = GameConstants.Title;
    public int Width { get; set; } = (int)GameConstants.DefaultWindowSize.X;
    public int Height { get; set; } = (int)GameConstants.DefaultWindowSize.Y;
    public bool Fullscreen { get; set; } = GameConstants.FullScreen;
    public bool VSync { get; set; } = GameConstants.VSync;
}
