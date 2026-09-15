namespace Markwardt.GodotServices.Internal;

/// <summary>
/// Populates <see cref="InjectAttribute"/>-marked public properties on nodes as they enter the
/// scene tree, resolving each property's value from the application's service provider.
/// </summary>
/// <param name="provider">The service provider used to resolve injected property values.</param>
internal sealed class NodeInjector(IServiceProvider provider)
{
    private readonly Dictionary<Type, PropertyInfo[]> injectablePropertiesByType = new();

    /// <summary>
    /// Populates every <see cref="InjectAttribute"/>-marked public property on <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node to inject dependencies into.</param>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="node"/>'s type marks a non-public property with <see cref="InjectAttribute"/>.
    /// </exception>
    internal void Inject(Node node)
    {
        foreach (PropertyInfo property in GetInjectableProperties(node.GetType()))
        {
            object? key = property.GetCustomAttribute<InjectAttribute>()!.Key;
            object value = key is null
                ? provider.GetRequiredService(property.PropertyType)
                : provider.GetRequiredKeyedService(property.PropertyType, key);
            property.SetValue(node, value);
        }
    }

    /// <summary>
    /// Gets every <see cref="InjectAttribute"/>-marked property on <paramref name="type"/>,
    /// caching the result for subsequent calls.
    /// </summary>
    /// <param name="type">The type to scan for <see cref="InjectAttribute"/>-marked properties.</param>
    /// <returns><paramref name="type"/>'s <see cref="InjectAttribute"/>-marked properties.</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="type"/> marks a non-public property with <see cref="InjectAttribute"/>.
    /// </exception>
    internal PropertyInfo[] GetInjectableProperties(Type type)
    {
        if (injectablePropertiesByType.TryGetValue(type, out PropertyInfo[]? cached))
        {
            return cached;
        }

        PropertyInfo[] decorated = type
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(property => property.IsDefined(typeof(InjectAttribute)))
            .ToArray();

        PropertyInfo? nonPublic = decorated.FirstOrDefault(property =>
            property.GetMethod is not { IsPublic: true } || property.SetMethod is not { IsPublic: true });

        if (nonPublic is not null)
        {
            throw new InvalidOperationException($"'{type.Name}.{nonPublic.Name}' is marked with [Inject] but is not a public property.");
        }

        injectablePropertiesByType[type] = decorated;
        return decorated;
    }
}
