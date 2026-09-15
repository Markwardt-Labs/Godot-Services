namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal, non-rebindable <see cref="IInputAxis"/> used to test <see cref="ServiceCollectionExtensions.AddInputAxis(IServiceCollection, object, Func{IServiceProvider, IInputAxis})"/>.
/// </summary>
internal sealed class FakeInputAxis : IInputAxis
{
    /// <inheritdoc />
    public float Value => 0f;
}
