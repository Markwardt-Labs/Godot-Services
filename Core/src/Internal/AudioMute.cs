namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes whether an audio bus is
/// muted via the engine's static <see cref="AudioServer"/> API, which cannot be substituted in a
/// test.
[ExcludeFromCodeCoverage]
internal sealed class AudioMute : IMutableSetting<bool>
{
    private readonly StringName bus;

    /// <param name="bus">The name of the audio bus.</param>
    internal AudioMute(string bus) => this.bus = bus;

    /// <inheritdoc />
    public bool Get() => AudioServer.IsBusMute(AudioServer.GetBusIndex(bus));

    /// <inheritdoc />
    public void Set(bool value) => AudioServer.SetBusMute(AudioServer.GetBusIndex(bus), value);
}
