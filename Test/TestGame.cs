using GameEngine;
using GameEngine.Logging;

namespace Test;
public class TestGame : Game
{
    public TestGame(string title) : base(title)
    {
        Run();
    }

    protected override void OnLoad()
        => Logger.GameLogger.Info("Loading Game");

    protected override void OnUnload()
        => Logger.GameLogger.Info("Unloading Game");

    protected override void Update(double deltaTime)
    {
        //Logger.GameLogger.Trace("Updating Game: " + deltaTime);
    }

    protected override void Render(double deltaTime)
    {
        // Logger.GameLogger.Trace("Rendering Game: " + deltaTime);
    }

    protected override void OnExit()
        => Logger.GameLogger.Info("Exiting Game");
}
