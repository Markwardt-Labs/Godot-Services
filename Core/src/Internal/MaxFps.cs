namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the engine's maximum
/// frames-per-second cap via the engine's static <see cref="Engine"/> API, which cannot be
/// substituted in a test.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class MaxFps : IMutableSetting<int>
{
    /// <inheritdoc />
    public int Get() => Engine.MaxFps;

    /// <inheritdoc />
    public void Set(int value) => Engine.MaxFps = value;
}
