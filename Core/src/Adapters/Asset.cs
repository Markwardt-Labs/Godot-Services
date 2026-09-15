namespace Markwardt.GodotServices;

/// <summary>
/// A lazily-loadable asset.
/// </summary>
/// <typeparam name="T">The type this asset loads as.</typeparam>
public interface IAsset<T>
{
    /// <summary>
    /// Loads the asset asynchronously.
    /// </summary>
    /// <returns>The loaded asset.</returns>
    Task<T> Load();
}
