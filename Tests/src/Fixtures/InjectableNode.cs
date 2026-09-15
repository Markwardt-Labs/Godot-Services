namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A <see cref="Node"/> fixture with an <see cref="InjectAttribute"/>-marked property, used to
/// test <see cref="NodeInjector.Inject"/>. Never constructed with <c>new()</c> - see
/// <see cref="GodotObjects.CreateUninitialized{T}"/>.
/// </summary>
internal sealed partial class InjectableNode : Node
{
    /// <summary>
    /// An <see cref="InjectAttribute"/>-marked property that should be populated by <see cref="NodeInjector.Inject"/>.
    /// </summary>
    [Inject]
    public required object Value { get; init; }
}
