namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A <see cref="Node"/> fixture with a keyed <see cref="InjectAttribute"/>-marked property, used
/// to test <see cref="NodeInjector.Inject"/>'s keyed service resolution. Never constructed with
/// <c>new()</c> - see <see cref="GodotObjects.CreateUninitialized{T}"/>.
/// </summary>
internal sealed partial class KeyedInjectableNode : Node
{
    /// <summary>
    /// A keyed <see cref="InjectAttribute"/>-marked property that should be populated by
    /// <see cref="NodeInjector.Inject"/> from the matching keyed service registration.
    /// </summary>
    [Inject("Key")]
    public required object Value { get; init; }
}
