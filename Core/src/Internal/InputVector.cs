namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IRebindableInputVector" path="/summary"/> Reads and rebinds four input
/// actions via the engine's static <see cref="Godot.Input"/>/<see cref="InputMap"/> APIs, which
/// cannot be substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class InputVector : IRebindableInputVector
{
    private readonly StringName negativeX;
    private readonly StringName positiveX;
    private readonly StringName negativeY;
    private readonly StringName positiveY;
    private readonly float deadzone;

    /// <param name="negativeX">The action for negative movement along the X axis.</param>
    /// <param name="positiveX">The action for positive movement along the X axis.</param>
    /// <param name="negativeY">The action for negative movement along the Y axis.</param>
    /// <param name="positiveY">The action for positive movement along the Y axis.</param>
    /// <param name="deadzone">
    /// The deadzone to apply, or a negative value to use the average of the four actions' own deadzones.
    /// </param>
    internal InputVector(string negativeX, string positiveX, string negativeY, string positiveY, float deadzone = -1f)
    {
        this.negativeX = negativeX;
        this.positiveX = positiveX;
        this.negativeY = negativeY;
        this.positiveY = positiveY;
        this.deadzone = deadzone;
    }

    /// <inheritdoc />
    public Vector2 Value => Godot.Input.GetVector(negativeX, positiveX, negativeY, positiveY, deadzone);

    /// <inheritdoc />
    public void RebindNegativeX(InputEvent @event)
    {
        InputMap.ActionEraseEvents(negativeX);
        InputMap.ActionAddEvent(negativeX, @event);
    }

    /// <inheritdoc />
    public void RebindPositiveX(InputEvent @event)
    {
        InputMap.ActionEraseEvents(positiveX);
        InputMap.ActionAddEvent(positiveX, @event);
    }

    /// <inheritdoc />
    public void RebindNegativeY(InputEvent @event)
    {
        InputMap.ActionEraseEvents(negativeY);
        InputMap.ActionAddEvent(negativeY, @event);
    }

    /// <inheritdoc />
    public void RebindPositiveY(InputEvent @event)
    {
        InputMap.ActionEraseEvents(positiveY);
        InputMap.ActionAddEvent(positiveY, @event);
    }
}
