namespace Markwardt.GodotServices;

/// <summary>
/// Reads the current time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// The number of milliseconds since the engine started, monotonically increasing.
    /// </summary>
    ulong ElapsedMilliseconds { get; }

    /// <summary>
    /// The current wall-clock time, in seconds since the Unix epoch.
    /// </summary>
    double UnixTime { get; }
}
