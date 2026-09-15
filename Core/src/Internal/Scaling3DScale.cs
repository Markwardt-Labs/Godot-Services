namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the root
/// <see cref="Viewport"/>'s 3D resolution scale factor, which cannot be substituted in a test.
/// </summary>
/// <param name="viewport">The root viewport to read/write the setting on.</param>
[ExcludeFromCodeCoverage]
internal sealed class Scaling3DScale(Viewport viewport) : IMutableSetting<float>
{
    /// <inheritdoc />
    public float Get() => viewport.Scaling3DScale;

    /// <inheritdoc />
    public void Set(float value) => viewport.Scaling3DScale = value;
}
