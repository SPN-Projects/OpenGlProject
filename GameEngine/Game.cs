using GameEngine.Graphics.Rendering;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GameEngine;

public abstract class Game
{
    private readonly string _windowTitle;
    private readonly GameWindow _nativeWindow;

    /// <summary>
    /// Create a new Game with a title
    /// Initialize the GameWindow with default settings
    /// Subscribe to the Window Events
    /// </summary>
    /// <param name="title"></param>
    public Game(string title, GameWindowSettings? gameWindowSettings, NativeWindowSettings? nativeWindowSettings)
    {
        _windowTitle = title;
        var defaultGameWindowSettings = gameWindowSettings ?? GameWindowSettings.Default;
        var defaultNativeWindowSettings = nativeWindowSettings ?? NativeWindowSettings.Default;

        _nativeWindow = new GameWindow(defaultGameWindowSettings, defaultNativeWindowSettings);

        // Window Event Handler
        _nativeWindow.Load += () =>
        {
            Renderer.Init();
            OnLoad();
        };

        _nativeWindow.UpdateFrame += (args) => Update(args.Time);
        _nativeWindow.RenderFrame += (args) =>
        {
            Render(args.Time);
            SwapBuffers();
        };

        _nativeWindow.FramebufferResize += (args) =>
        {
            if (args.Width != 0 || args.Height != 0)
            {
                Logger.EngineLogger.Trace($"Resizing Window to {args.Width}x{args.Height}");
                OnResize(args.Width, args.Height);
            }
            else
            {
                Logger.EngineLogger.Trace("Window minimized");
            }
        };

        _nativeWindow.Closing += (args) =>
        {
            OnUnload();
            OnExit();
        };

        // Window API and GPU Version Logging
        Logger.EngineLogger.Info("Window Created with folwing settings:");
        Logger.EngineLogger.Info($"Title: {_windowTitle}");
        Logger.EngineLogger.Info($"Resolution: {_nativeWindow.ClientSize}" + "\n");

        Logger.EngineLogger.Info("OpenGL Context Created");
        Logger.EngineLogger.Info("GPU: " + GL.GetString(StringName.Renderer));
        Logger.EngineLogger.Info("OpenGL Version: " + GL.GetString(StringName.Version));
        Logger.EngineLogger.Info("GLSL Version: " + GL.GetString(StringName.ShadingLanguageVersion) + "\n");

        Logger.EngineLogger.Info("API: " + _nativeWindow.API);
        Logger.EngineLogger.Info("API Version: " + _nativeWindow.APIVersion + "\n");
    }

    protected void Run()
        => _nativeWindow.Run();

    protected void Close()
        => _nativeWindow.Close();

    protected void ToggleWindowVisibility(bool isVisible)
        => _nativeWindow.IsVisible = isVisible;

    protected void ToggleFullscreen(bool isFullScreen)
        => _nativeWindow.WindowState = isFullScreen ? WindowState.Fullscreen : WindowState.Normal;

    protected void ToggleVSync(bool isVSync)
        => _nativeWindow.VSync = isVSync ? VSyncMode.On : VSyncMode.Off;

    protected abstract void Update(double deltaTime);
    protected abstract void Render(double deltaTime);
    protected abstract void OnResize(int width, int height);

    protected abstract void OnLoad();
    protected abstract void OnUnload();
    protected abstract void OnExit();

    protected void SwapBuffers()
        => _nativeWindow.SwapBuffers();
}
