using GameEngine.Graphics.Rendering;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine;

public abstract class Game
{
    private readonly string _windowTitle;
    protected GameWindow NativeWindow { get; private set; }
    protected KeyboardState KeyboardState => NativeWindow.KeyboardState;
    protected MouseState MouseState => NativeWindow.MouseState;

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

        NativeWindow = new GameWindow(defaultGameWindowSettings, defaultNativeWindowSettings);
        // Window Event Handler

        Renderer.Init();
        NativeWindow.Load += OnLoad;

        NativeWindow.UpdateFrame += (args) =>
        {
            NativeWindow.Title = $"{_windowTitle} - FPS: {1 / args.Time:0.00}";
            Update(args.Time);
        };

        NativeWindow.RenderFrame += (args) =>
        {
            Render(args.Time);
            SwapBuffers();
        };

        NativeWindow.FramebufferResize += (args) =>
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

        NativeWindow.Closing += (args) =>
        {
            OnUnload();
            OnExit();
        };

        // Window API and GPU Version Logging
        Logger.EngineLogger.Info("Window Created with folwing settings:");
        Logger.EngineLogger.Info($"Title: {_windowTitle}");
        Logger.EngineLogger.Info($"Resolution: {NativeWindow.ClientSize}" + "\n");

        Logger.EngineLogger.Info("OpenGL Context Created");
        Logger.EngineLogger.Info("GPU: " + GL.GetString(StringName.Renderer));
        Logger.EngineLogger.Info("OpenGL Version: " + GL.GetString(StringName.Version));
        Logger.EngineLogger.Info("GLSL Version: " + GL.GetString(StringName.ShadingLanguageVersion) + "\n");

        Logger.EngineLogger.Info("API: " + NativeWindow.API);
        Logger.EngineLogger.Info("API Version: " + NativeWindow.APIVersion + "\n");
    }

    protected void Run()
        => NativeWindow.Run();

    protected void Close()
        => NativeWindow.Close();

    protected void ToggleWindowVisibility(bool isVisible)
        => NativeWindow.IsVisible = isVisible;

    protected void ToggleFullscreen(bool isFullScreen)
        => NativeWindow.WindowState = isFullScreen ? WindowState.Fullscreen : WindowState.Normal;

    protected void ToggleVSync(bool isVSync)
        => NativeWindow.VSync = isVSync ? VSyncMode.On : VSyncMode.Off;

    protected abstract void Update(double deltaTime);
    protected abstract void Render(double deltaTime);
    protected abstract void OnResize(int width, int height);

    protected abstract void OnLoad();
    protected abstract void OnUnload();
    protected abstract void OnExit();

    protected void SwapBuffers()
        => NativeWindow.SwapBuffers();
}
