namespace Markwardt.GodotServices;

/// <summary>
/// A lazily-loadable packed scene that can be instantiated or opened as the active scene.
/// </summary>
public interface ISceneAsset : IAsset<PackedScene>
{
    /// <summary>
    /// Loads the packed scene and instantiates its node hierarchy.
    /// </summary>
    /// <typeparam name="TNode">The root node type to instantiate the scene as.</typeparam>
    /// <returns>The instantiated root node.</returns>
    Task<TNode> Instantiate<TNode>()
        where TNode : Node;

    /// <summary>
    /// Loads the packed scene and changes the active scene to it.
    /// </summary>
    Task Open();
}
