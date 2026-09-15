namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the root
/// <see cref="Viewport"/>'s 3D resolution scaling mode, which cannot be substituted in a test.
/// </summary>
/// <param name="viewport">The root viewport to read/write the setting on.</param>
[ExcludeFromCodeCoverage]
internal sealed class Scaling3DMode(Viewport viewport) : IMutableSetting<Viewport.Scaling3DModeEnum>
{
    /// <inheritdoc />
    public Viewport.Scaling3DModeEnum Get() => viewport.Scaling3DMode;

    /// <inheritdoc />
    public void Set(Viewport.Scaling3DModeEnum value) => viewport.Scaling3DMode = value;
}
