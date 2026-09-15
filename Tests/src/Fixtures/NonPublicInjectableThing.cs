namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A fixture whose <see cref="InjectAttribute"/>-marked property has a non-public setter, used to
/// test <see cref="NodeInjector.GetInjectableProperties"/>'s validation failure path.
/// </summary>
internal sealed class NonPublicInjectableThing
{
    /// <summary>
    /// An <see cref="InjectAttribute"/>-marked property that is not fully public.
    /// </summary>
    [Inject]
    public required object Invalid { get; internal init; }
}
