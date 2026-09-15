namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A plain (non-<see cref="Node"/>) fixture used to test <see cref="NodeInjector.GetInjectableProperties"/>.
/// </summary>
internal sealed class InjectableThing
{
    /// <summary>
    /// An <see cref="InjectAttribute"/>-marked property that should be discovered.
    /// </summary>
    [Inject]
    public required object First { get; init; }

    /// <summary>
    /// A second <see cref="InjectAttribute"/>-marked property that should be discovered.
    /// </summary>
    [Inject]
    public required object Second { get; init; }

    /// <summary>
    /// A property with no <see cref="InjectAttribute"/> that should be ignored.
    /// </summary>
    public object NotInjected { get; set; } = null!;
}
