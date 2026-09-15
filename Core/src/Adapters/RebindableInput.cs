namespace Markwardt.GodotServices;

/// <summary>
/// <inheritdoc cref="IInput" path="/summary"/> Also allows the action's bound input event to be
/// changed at runtime.
/// </summary>
public interface IRebindableInput : IInput
{
    /// <summary>
    /// The action's deadzone - the minimum <see cref="IInput.Strength"/> before the action is
    /// considered pressed at all.
    /// </summary>
    float Deadzone { get; set; }

    /// <summary>
    /// Replaces every input event currently bound to the action with <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void Rebind(InputEvent @event);
}
