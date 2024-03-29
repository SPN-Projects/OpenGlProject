using GameEngine.Extensions;
using GameEngine.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GameEngine;

public abstract class Game
{
    private readonly string _windowTitle;
    private readonly GameWindow _nativeWindow;
    private readonly bool _disposedValue;

    /// <summary>
    /// Create a new Game with a title
    /// Initialize the GameWindow with default settings
    /// Subscribe to the Window Events
    /// </summary>
    /// <param name="title"></param>
    public Game(string title)
    {
        _windowTitle = title;
        var defaultGameWindowSettings = GetDefaultGameWindowSettings();
        var defaultNativeWindowSettings = GetDefaultNativeWindowSettings();

        _nativeWindow = new GameWindow(defaultGameWindowSettings, defaultNativeWindowSettings);

        // Window Event Handler
        _nativeWindow.Load += OnLoad;
        _nativeWindow.UpdateFrame += (args) => Update(args.Time);
        _nativeWindow.RenderFrame += (args) => Render(args.Time);
        _nativeWindow.FramebufferResize += (args) => OnResize(args.Width, args.Height);
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

    protected abstract void Update(double deltaTime);
    protected abstract void Render(double deltaTime);
    protected abstract void OnResize(int width, int height);

    protected abstract void OnLoad();
    protected abstract void OnUnload();
    protected abstract void OnExit();

    protected void SwapBuffers()
        => _nativeWindow.SwapBuffers();


    /// <summary>
    /// Get the customized default game window settings for the OpenTK GameWindow
    /// </summary>
    /// <returns>Our custom default <see cref="GameWindowSettings"/> Settings</returns>
    private static GameWindowSettings GetDefaultGameWindowSettings()
    {
        var windowSettings = new GameWindowSettings
        {
            UpdateFrequency = 60,
        };

        return windowSettings;
    }

    /// <summary>
    /// Get our customized default native window settings for the OpenTK GameWindow
    /// </summary>
    /// <returns>Our custom default <see cref="NativeWindowSettings"/> Settings</returns>
    private NativeWindowSettings GetDefaultNativeWindowSettings()
    {
        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = EngineConstants.DefaultWindowSize.ToOpenTKi(),
            Title = _windowTitle,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6),
        };

        return nativeWindowSettings;
    }
}
