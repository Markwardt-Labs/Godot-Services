namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the root
/// <see cref="Window"/>'s UI scale factor, which cannot be substituted in a test.
/// </summary>
/// <param name="window">The root window to read/write the setting on.</param>
[ExcludeFromCodeCoverage]
internal sealed class ContentScaleFactor(Window window) : IMutableSetting<float>
{
    /// <inheritdoc />
    public float Get() => window.ContentScaleFactor;

    /// <inheritdoc />
    public void Set(float value) => window.ContentScaleFactor = value;
}
