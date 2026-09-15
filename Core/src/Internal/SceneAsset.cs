namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="ISceneAsset" />
internal sealed class SceneAsset(IAssetLoader assetLoader, ISceneNavigator sceneNavigator, string path)
    : ResourceAsset<PackedScene>(assetLoader, path), ISceneAsset
{
    /// <inheritdoc />
    public async Task<TNode> Instantiate<TNode>()
        where TNode : Node
        => (await Load()).Instantiate<TNode>();

    /// <inheritdoc />
    public async Task Open() => sceneNavigator.ChangeScene(await Load());
}
