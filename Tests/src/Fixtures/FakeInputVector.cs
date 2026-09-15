namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal, non-rebindable <see cref="IInputVector"/> used to test <see cref="ServiceCollectionExtensions.AddInputVector(IServiceCollection, object, Func{IServiceProvider, IInputVector})"/>.
/// </summary>
internal sealed class FakeInputVector : IInputVector
{
    /// <inheritdoc />
    public Vector2 Value => Vector2.Zero;
}
