namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the main window's
/// fullscreen/windowed display mode via the engine's static <see cref="DisplayServer"/> API,
/// which cannot be substituted in a test.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class WindowMode : IMutableSetting<DisplayServer.WindowMode>
{
    /// <inheritdoc />
    public DisplayServer.WindowMode Get() => DisplayServer.WindowGetMode();

    /// <inheritdoc />
    public void Set(DisplayServer.WindowMode value) => DisplayServer.WindowSetMode(value);
}
