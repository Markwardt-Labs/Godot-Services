namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A fixture whose <see cref="InjectAttribute"/>-marked property has a non-public getter, used to
/// test <see cref="NodeInjector.GetInjectableProperties"/>'s validation failure path for the
/// getter side specifically, as distinct from <see cref="NonPublicInjectableThing"/>'s
/// setter-side check.
/// </summary>
internal sealed class NonPublicGetterInjectableThing
{
    /// <summary>
    /// An <see cref="InjectAttribute"/>-marked property whose getter is not public.
    /// </summary>
    [Inject]
    public required object Invalid { private get; init; }
}
