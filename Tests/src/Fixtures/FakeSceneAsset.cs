namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="ISceneAsset"/> used to test <see cref="ServiceCollectionExtensions.AddScene(IServiceCollection, object, Func{IServiceProvider, ISceneAsset})"/>.
/// </summary>
internal sealed class FakeSceneAsset : ISceneAsset
{
    /// <inheritdoc />
    public Task<PackedScene> Load() => Task.FromResult<PackedScene>(null!);

    /// <inheritdoc />
    public Task<TNode> Instantiate<TNode>()
        where TNode : Node
        => Task.FromResult<TNode>(null!);

    /// <inheritdoc />
    public Task Open() => Task.CompletedTask;
}
