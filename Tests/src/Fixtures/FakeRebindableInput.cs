namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="IRebindableInput"/> used to test <see cref="ServiceCollectionExtensions.AddInput(IServiceCollection, object, Func{IServiceProvider, IRebindableInput})"/>.
/// </summary>
internal sealed class FakeRebindableInput : IRebindableInput
{
    /// <inheritdoc />
    public float Strength => 0f;

    /// <inheritdoc />
    public float Deadzone { get; set; }

    /// <inheritdoc />
    public bool IsPressed() => false;

    /// <inheritdoc />
    public bool IsJustPressed() => false;

    /// <inheritdoc />
    public bool IsJustReleased() => false;

    /// <inheritdoc />
    public void Rebind(InputEvent @event)
    {
    }
}
