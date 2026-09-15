namespace Markwardt.GodotServices;

/// <summary>
/// <inheritdoc cref="IInputAxis" path="/summary"/> Also allows each of the two bound actions'
/// input events to be changed at runtime.
/// </summary>
public interface IRebindableInputAxis : IInputAxis
{
    /// <summary>
    /// Replaces every input event currently bound to the negative action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindNegative(InputEvent @event);

    /// <summary>
    /// Replaces every input event currently bound to the positive action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindPositive(InputEvent @event);
}
