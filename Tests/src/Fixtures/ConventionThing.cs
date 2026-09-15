namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A convention-named contract used to test <see cref="ServiceCollectionExtensions.AddConventionServices"/>.
/// </summary>
internal interface IConventionThing;

/// <inheritdoc cref="IConventionThing" />
internal sealed class ConventionThing : IConventionThing;
