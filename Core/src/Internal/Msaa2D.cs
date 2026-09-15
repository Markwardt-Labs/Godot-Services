namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the root
/// <see cref="Viewport"/>'s 2D MSAA quality, which cannot be substituted in a test.
/// </summary>
/// <param name="viewport">The root viewport to read/write the setting on.</param>
[ExcludeFromCodeCoverage]
internal sealed class Msaa2D(Viewport viewport) : IMutableSetting<Viewport.Msaa>
{
    /// <inheritdoc />
    public Viewport.Msaa Get() => viewport.Msaa2D;

    /// <inheritdoc />
    public void Set(Viewport.Msaa value) => viewport.Msaa2D = value;
}
