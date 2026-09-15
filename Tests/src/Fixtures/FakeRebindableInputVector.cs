namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="IRebindableInputVector"/> used to test <see cref="ServiceCollectionExtensions.AddInputVector(IServiceCollection, object, Func{IServiceProvider, IRebindableInputVector})"/>.
/// </summary>
internal sealed class FakeRebindableInputVector : IRebindableInputVector
{
    /// <inheritdoc />
    public Vector2 Value => Vector2.Zero;

    /// <inheritdoc />
    public void RebindNegativeX(InputEvent @event)
    {
    }

    /// <inheritdoc />
    public void RebindPositiveX(InputEvent @event)
    {
    }

    /// <inheritdoc />
    public void RebindNegativeY(InputEvent @event)
    {
    }

    /// <inheritdoc />
    public void RebindPositiveY(InputEvent @event)
    {
    }
}
