namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the index of the screen
/// the main window is currently on via the engine's static <see cref="DisplayServer"/> API, which
/// cannot be substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class CurrentScreen : IMutableSetting<int>
{
    /// <inheritdoc />
    public int Get() => DisplayServer.WindowGetCurrentScreen();

    /// <inheritdoc />
    public void Set(int value) => DisplayServer.WindowSetCurrentScreen(value);
}
