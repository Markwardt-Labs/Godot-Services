namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IRebindableInputAxis" path="/summary"/> Reads and rebinds two input actions
/// via the engine's static <see cref="Godot.Input"/>/<see cref="InputMap"/> APIs, which cannot be
/// substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class InputAxis : IRebindableInputAxis
{
    private readonly StringName negativeAction;
    private readonly StringName positiveAction;

    /// <param name="negativeAction">The action for the negative direction.</param>
    /// <param name="positiveAction">The action for the positive direction.</param>
    internal InputAxis(string negativeAction, string positiveAction)
    {
        this.negativeAction = negativeAction;
        this.positiveAction = positiveAction;
    }

    /// <inheritdoc />
    public float Value => Godot.Input.GetAxis(negativeAction, positiveAction);

    /// <inheritdoc />
    public void RebindNegative(InputEvent @event)
    {
        InputMap.ActionEraseEvents(negativeAction);
        InputMap.ActionAddEvent(negativeAction, @event);
    }

    /// <inheritdoc />
    public void RebindPositive(InputEvent @event)
    {
        InputMap.ActionEraseEvents(positiveAction);
        InputMap.ActionAddEvent(positiveAction, @event);
    }
}
