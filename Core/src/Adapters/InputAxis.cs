namespace Markwardt.GodotServices;

/// <summary>
/// A bound, reusable 1D directional input reading, combining two opposing input actions whose
/// names are already known, so callers don't thread action name strings through their own code.
/// </summary>
public interface IInputAxis
{
    /// <summary>
    /// The current axis value, from <c>-1</c> (fully negative) to <c>1</c> (fully positive).
    /// </summary>
    float Value { get; }
}
