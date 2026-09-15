namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal, non-rebindable <see cref="IInput"/> used to test <see cref="ServiceCollectionExtensions.AddInput(IServiceCollection, object, Func{IServiceProvider, IInput})"/>.
/// </summary>
internal sealed class FakeInput : IInput
{
    /// <inheritdoc />
    public float Strength => 0f;

    /// <inheritdoc />
    public bool IsPressed() => false;

    /// <inheritdoc />
    public bool IsJustPressed() => false;

    /// <inheritdoc />
    public bool IsJustReleased() => false;
}
