namespace Markwardt.GodotServices;

/// <summary>
/// A bound, reusable setting - a setting whose name is already known, so callers don't thread a
/// name string through their own code.
/// </summary>
/// <typeparam name="T">The type of the setting's value.</typeparam>
public interface ISetting<T>
{
    /// <summary>
    /// Gets the setting's current value.
    /// </summary>
    /// <returns>The setting's current value.</returns>
    T Get();
}
