namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IClock" path="/summary"/> Wraps the engine's static <see cref="Time"/> API,
/// which cannot be substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class Clock : IClock
{
    /// <inheritdoc />
    public ulong ElapsedMilliseconds => Time.GetTicksMsec();

    /// <inheritdoc />
    public double UnixTime => Time.GetUnixTimeFromSystem();
}
