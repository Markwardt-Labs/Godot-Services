namespace Markwardt.GodotServices;

/// <summary>
/// A bound, reusable query against a single input action - an input action whose name is already
/// known, so callers don't thread an action name string through their own code.
/// </summary>
public interface IInput
{
    /// <summary>
    /// The action's current analog strength, from <c>0</c> (not pressed) to <c>1</c> (fully pressed).
    /// </summary>
    float Strength { get; }

    /// <summary>
    /// Whether the action is currently pressed.
    /// </summary>
    /// <returns><see langword="true"/> if the action is currently pressed.</returns>
    bool IsPressed();

    /// <summary>
    /// Whether the action was pressed during this frame.
    /// </summary>
    /// <returns><see langword="true"/> if the action was pressed during this frame.</returns>
    bool IsJustPressed();

    /// <summary>
    /// Whether the action was released during this frame.
    /// </summary>
    /// <returns><see langword="true"/> if the action was released during this frame.</returns>
    bool IsJustReleased();
}
