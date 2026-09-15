namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IAsset{T}" path="/summary"/> Loads <paramref name="path"/> as a
/// <typeparamref name="TResource"/> via <paramref name="assetLoader"/>.
/// </summary>
/// <typeparam name="TResource">The resource type this asset loads as.</typeparam>
/// <param name="assetLoader">The asset loader to load the resource with.</param>
/// <param name="path">The <c>res://</c> path to the resource.</param>
internal class ResourceAsset<TResource>(IAssetLoader assetLoader, string path) : IAsset<TResource>
    where TResource : Resource
{
    /// <summary>
    /// The <c>res://</c> path to the resource.
    /// </summary>
    protected string Path { get; } = path;

    /// <inheritdoc />
    public Task<TResource> Load() => assetLoader.Load<TResource>(Path);
}
