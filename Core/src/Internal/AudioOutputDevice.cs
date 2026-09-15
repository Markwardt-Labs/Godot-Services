namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the active audio output
/// device via the engine's static <see cref="AudioServer"/> API, which cannot be substituted in a
/// test.
[ExcludeFromCodeCoverage]
internal sealed class AudioOutputDevice : IMutableSetting<string>
{
    /// <inheritdoc />
    public string Get() => AudioServer.OutputDevice;

    /// <inheritdoc />
    public void Set(string value) => AudioServer.OutputDevice = value;
}
