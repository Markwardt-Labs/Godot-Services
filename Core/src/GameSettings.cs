namespace Markwardt.GodotServices;

/// <summary>
/// The keys identifying this library's own engine-backed <see cref="IMutableSetting{T}"/>
/// registrations, registered automatically by <see cref="ServiceContainer"/>. Declared as
/// <see langword="const"/> fields, rather than this project's usual static-property convention
/// for fixed values, since <see cref="InjectAttribute"/> is only able to accept compile-time
/// constants as constructor arguments. Prefer <see cref="Markwardt.GodotServices.ProjectSettings"/>
/// instead for a setting that only needs to be read once, at startup, rather than changed live.
/// </summary>
public static class GameSettings
{
    /// <summary>
    /// The key for the main window's fullscreen/windowed display mode, an
    /// <see cref="IMutableSetting{T}"/> of <see cref="Godot.DisplayServer.WindowMode"/>.
    /// </summary>
    public const string WindowMode = "WindowMode";

    /// <summary>
    /// The key for the main window's vertical sync mode, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Godot.DisplayServer.VSyncMode"/>.
    /// </summary>
    public const string VSyncMode = "VSyncMode";

    /// <summary>
    /// The key for the main window's size, in pixels, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Vector2I"/>.
    /// </summary>
    public const string WindowSize = "WindowSize";

    /// <summary>
    /// The key for the index of the screen the main window is currently on, an
    /// <see cref="IMutableSetting{T}"/> of <see langword="int"/>.
    /// </summary>
    public const string CurrentScreen = "CurrentScreen";

    /// <summary>
    /// The key for the 2D MSAA quality, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Viewport.Msaa"/>.
    /// </summary>
    public const string Msaa2D = "Msaa2D";

    /// <summary>
    /// The key for the 3D MSAA quality, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Viewport.Msaa"/>.
    /// </summary>
    public const string Msaa3D = "Msaa3D";

    /// <summary>
    /// The key for the screen-space anti-aliasing mode, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Viewport.ScreenSpaceAAEnum"/>.
    /// </summary>
    public const string ScreenSpaceAA = "ScreenSpaceAA";

    /// <summary>
    /// The key for the 3D resolution scaling mode, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Viewport.Scaling3DModeEnum"/>.
    /// </summary>
    public const string Scaling3DMode = "Scaling3DMode";

    /// <summary>
    /// The key for the 3D resolution scale factor, an <see cref="IMutableSetting{T}"/> of
    /// <see langword="float"/>.
    /// </summary>
    public const string Scaling3DScale = "Scaling3DScale";

    /// <summary>
    /// The key for the main window's UI scale factor, an <see cref="IMutableSetting{T}"/> of
    /// <see langword="float"/>.
    /// </summary>
    public const string ContentScaleFactor = "ContentScaleFactor";

    /// <summary>
    /// The key for the name of the audio output device in use, an <see cref="IMutableSetting{T}"/> of
    /// <see langword="string"/>.
    /// </summary>
    public const string AudioOutputDevice = "AudioOutputDevice";

    /// <summary>
    /// The key for the name of the audio input (microphone) device in use, an
    /// <see cref="IMutableSetting{T}"/> of <see langword="string"/>.
    /// </summary>
    public const string AudioInputDevice = "AudioInputDevice";

    /// <summary>
    /// The key for the mouse's capture/visibility mode, an <see cref="IMutableSetting{T}"/> of
    /// <see cref="Godot.Input.MouseModeEnum"/>.
    /// </summary>
    public const string MouseMode = "MouseMode";

    /// <summary>
    /// The key for the engine's maximum frames-per-second cap, an <see cref="IMutableSetting{T}"/> of
    /// <see langword="int"/>. A value of <c>0</c> means uncapped.
    /// </summary>
    public const string MaxFps = "MaxFps";

    /// <summary>
    /// The key for the active locale, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.
    /// </summary>
    public const string Locale = "Locale";
}
