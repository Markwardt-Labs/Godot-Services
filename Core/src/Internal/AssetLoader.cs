namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IAssetLoader" path="/summary"/> Wraps the engine's static, poll-based
/// <see cref="ResourceLoader"/> API, which cannot be substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class AssetLoader : IAssetLoader
{
    private readonly TimeSpan pollInterval = TimeSpan.FromMilliseconds(10);

    /// <inheritdoc />
    public async Task<TResource> Load<TResource>(string path)
        where TResource : Resource
    {
        ResourceLoader.LoadThreadedRequest(path);

        ResourceLoader.ThreadLoadStatus status;
        while ((status = ResourceLoader.LoadThreadedGetStatus(path)) == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            await Task.Delay(pollInterval);
        }

        if (status != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            throw new InvalidOperationException($"Failed to load resource at '{path}': {status}.");
        }

        return (TResource)ResourceLoader.LoadThreadedGet(path);
    }
}
