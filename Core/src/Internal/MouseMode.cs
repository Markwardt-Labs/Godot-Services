namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the mouse's
/// capture/visibility mode via the engine's static <see cref="Godot.Input"/> API, which cannot be
/// substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class MouseMode : IMutableSetting<Godot.Input.MouseModeEnum>
{
    /// <inheritdoc />
    public Godot.Input.MouseModeEnum Get() => Godot.Input.GetMouseMode();

    /// <inheritdoc />
    public void Set(Godot.Input.MouseModeEnum value) => Godot.Input.SetMouseMode(value);
}
