namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A convention-named contract used to test <see cref="NodeSingletonRegistry"/> honoring
/// <see cref="TransientAttribute"/>.
/// </summary>
internal interface ITransientConventionNode;

/// <inheritdoc cref="ITransientConventionNode" />
[Transient]
internal sealed partial class TransientConventionNode : Node, ITransientConventionNode;
