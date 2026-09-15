namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="IRebindableInputAxis"/> used to test <see cref="ServiceCollectionExtensions.AddInputAxis(IServiceCollection, object, Func{IServiceProvider, IRebindableInputAxis})"/>.
/// </summary>
internal sealed class FakeRebindableInputAxis : IRebindableInputAxis
{
    /// <inheritdoc />
    public float Value => 0f;

    /// <inheritdoc />
    public void RebindNegative(InputEvent @event)
    {
    }

    /// <inheritdoc />
    public void RebindPositive(InputEvent @event)
    {
    }
}
