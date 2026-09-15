namespace Markwardt.GodotServices.Internal;

/// <summary>
/// Discovers assemblies marked with <see cref="ConventionScannableAttribute"/>.
/// </summary>
internal static class ConventionScannableAssemblies
{
    /// <summary>
    /// Gets every currently loaded assembly marked with <see cref="ConventionScannableAttribute"/>.
    /// </summary>
    /// <returns>The currently loaded, convention-scannable assemblies.</returns>
    internal static IEnumerable<Assembly> Discover() =>
        AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDefined(typeof(ConventionScannableAttribute)));
}
