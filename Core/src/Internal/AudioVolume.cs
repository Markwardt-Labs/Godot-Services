namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes an audio bus's volume, in
/// decibels, via the engine's static <see cref="AudioServer"/> API, which cannot be substituted
/// in a test.
[ExcludeFromCodeCoverage]
internal sealed class AudioVolume : IMutableSetting<float>
{
    private readonly StringName bus;

    /// <param name="bus">The name of the audio bus.</param>
    internal AudioVolume(string bus) => this.bus = bus;

    /// <inheritdoc />
    public float Get() => AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(bus));

    /// <inheritdoc />
    public void Set(float value) => AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(bus), value);
}
