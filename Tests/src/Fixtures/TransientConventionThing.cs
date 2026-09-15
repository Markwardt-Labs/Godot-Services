namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A convention-named contract used to test <see cref="ServiceCollectionExtensions.AddConventionServices"/>
/// honoring <see cref="TransientAttribute"/>.
/// </summary>
internal interface ITransientConventionThing;

/// <inheritdoc cref="ITransientConventionThing" />
[Transient]
internal sealed class TransientConventionThing : ITransientConventionThing;
