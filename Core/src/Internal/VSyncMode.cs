namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the main window's
/// vertical sync mode via the engine's static <see cref="DisplayServer"/> API, which cannot be
/// substituted in a test.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class VSyncMode : IMutableSetting<DisplayServer.VSyncMode>
{
    /// <inheritdoc />
    public DisplayServer.VSyncMode Get() => DisplayServer.WindowGetVsyncMode();

    /// <inheritdoc />
    public void Set(DisplayServer.VSyncMode value) => DisplayServer.WindowSetVsyncMode(value);
}
