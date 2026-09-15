namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A convention-named contract used to test <see cref="NodeSingletonRegistry"/>.
/// </summary>
internal interface IConventionNode;

/// <inheritdoc cref="IConventionNode" />
internal sealed partial class ConventionNode : Node, IConventionNode;
