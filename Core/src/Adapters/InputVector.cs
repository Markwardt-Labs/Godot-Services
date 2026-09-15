namespace Markwardt.GodotServices;

/// <summary>
/// A bound, reusable 2D directional input reading, combining four input actions - two opposing
/// pairs whose names are already known, so callers don't thread action name strings through
/// their own code.
/// </summary>
public interface IInputVector
{
    /// <summary>
    /// The current directional input, normalized so its length never exceeds <c>1</c>.
    /// </summary>
    Vector2 Value { get; }
}
