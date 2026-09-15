namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the root
/// <see cref="Viewport"/>'s screen-space anti-aliasing mode, which cannot be substituted in a
/// test.
/// </summary>
/// <param name="viewport">The root viewport to read/write the setting on.</param>
[ExcludeFromCodeCoverage]
internal sealed class ScreenSpaceAA(Viewport viewport) : IMutableSetting<Viewport.ScreenSpaceAAEnum>
{
    /// <inheritdoc />
    public Viewport.ScreenSpaceAAEnum Get() => viewport.ScreenSpaceAA;

    /// <inheritdoc />
    public void Set(Viewport.ScreenSpaceAAEnum value) => viewport.ScreenSpaceAA = value;
}
