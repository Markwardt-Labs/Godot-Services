namespace Markwardt.GodotServices;

/// <summary>
/// <inheritdoc cref="IInputVector" path="/summary"/> Also allows each of the four bound actions'
/// input events to be changed at runtime.
/// </summary>
public interface IRebindableInputVector : IInputVector
{
    /// <summary>
    /// Replaces every input event currently bound to the negative-X action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindNegativeX(InputEvent @event);

    /// <summary>
    /// Replaces every input event currently bound to the positive-X action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindPositiveX(InputEvent @event);

    /// <summary>
    /// Replaces every input event currently bound to the negative-Y action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindNegativeY(InputEvent @event);

    /// <summary>
    /// Replaces every input event currently bound to the positive-Y action with
    /// <paramref name="event"/>.
    /// </summary>
    /// <param name="event">The input event to bind.</param>
    void RebindPositiveY(InputEvent @event);
}
