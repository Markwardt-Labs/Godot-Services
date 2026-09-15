namespace Markwardt.GodotServices;

/// <summary>
/// Marks a type so that automatic convention-based registration registers it as transient
/// rather than as a singleton.
/// </summary>
/// <remarks>
/// Applies to convention-based registration performed by
/// <see cref="ServiceCollectionExtensions.AddConventionServices(IServiceCollection, Assembly)"/>
/// and node-backed registration performed internally by <see cref="ServiceContainer"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TransientAttribute : Attribute;
