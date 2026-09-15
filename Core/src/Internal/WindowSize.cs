namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the main window's size
/// via the engine's static <see cref="DisplayServer"/> API, which cannot be substituted in a
/// test.
[ExcludeFromCodeCoverage]
internal sealed class WindowSize : IMutableSetting<Vector2I>
{
    /// <inheritdoc />
    public Vector2I Get() => DisplayServer.WindowGetSize();

    /// <inheritdoc />
    public void Set(Vector2I value) => DisplayServer.WindowSetSize(value);
}
