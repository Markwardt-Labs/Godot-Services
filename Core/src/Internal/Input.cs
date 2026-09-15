namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IRebindableInput" path="/summary"/> Queries and rebinds an input action via
/// the engine's static <see cref="Godot.Input"/>/<see cref="InputMap"/> APIs, which cannot be
/// substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class Input : IRebindableInput
{
    private readonly StringName action;

    /// <param name="action">The name of the input action to query and rebind.</param>
    internal Input(string action) => this.action = action;

    /// <inheritdoc />
    public float Strength => Godot.Input.GetActionStrength(action);

    /// <inheritdoc />
    public float Deadzone
    {
        get => InputMap.ActionGetDeadzone(action);
        set => InputMap.ActionSetDeadzone(action, value);
    }

    /// <inheritdoc />
    public bool IsPressed() => Godot.Input.IsActionPressed(action);

    /// <inheritdoc />
    public bool IsJustPressed() => Godot.Input.IsActionJustPressed(action);

    /// <inheritdoc />
    public bool IsJustReleased() => Godot.Input.IsActionJustReleased(action);

    /// <inheritdoc />
    public void Rebind(InputEvent @event)
    {
        InputMap.ActionEraseEvents(action);
        InputMap.ActionAddEvent(action, @event);
    }
}
