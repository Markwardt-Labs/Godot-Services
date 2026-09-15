namespace Markwardt.GodotServices;

/// <summary>
/// Marks an assembly as eligible for automatic convention-based service scanning by
/// <see cref="ServiceContainer"/> and the no-argument
/// <see cref="ServiceCollectionExtensions.AddConventionServices(IServiceCollection)"/> overload.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ConventionScannableAttribute : Attribute;
