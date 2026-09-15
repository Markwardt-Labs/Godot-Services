namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the active audio input
/// (microphone) device via the engine's static <see cref="AudioServer"/> API, which cannot be
/// substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class AudioInputDevice : IMutableSetting<string>
{
    /// <inheritdoc />
    public string Get() => AudioServer.InputDevice;

    /// <inheritdoc />
    public void Set(string value) => AudioServer.InputDevice = value;
}
