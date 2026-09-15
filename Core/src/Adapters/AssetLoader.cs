namespace Markwardt.GodotServices;

/// <summary>
/// Loads engine resources without blocking the calling thread.
/// </summary>
public interface IAssetLoader
{
    /// <summary>
    /// Loads the resource at <paramref name="path"/> asynchronously.
    /// </summary>
    /// <typeparam name="TResource">The resource type to load.</typeparam>
    /// <param name="path">The <c>res://</c> path to the resource.</param>
    /// <returns>The loaded resource.</returns>
    /// <exception cref="InvalidOperationException">The resource failed to load.</exception>
    Task<TResource> Load<TResource>(string path)
        where TResource : Resource;
}
