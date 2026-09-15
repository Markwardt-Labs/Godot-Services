namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="SceneAsset"/>. <see cref="ISceneAsset.Instantiate{TNode}"/> isn't covered
/// here: it calls the real <c>PackedScene.Instantiate</c>, which needs a running Godot engine and
/// an actually-packed scene to do anything meaningful - see <c>Docs/Testing.md</c>.
/// </summary>
public sealed class SceneAssetTests
{
    [Fact]
    public async Task Open_ChangesTheSceneToTheLoadedPackedScene()
    {
        PackedScene packedScene = GodotObjects.CreateUninitialized<PackedScene>();
        Mock<IAssetLoader> assetLoader = new();
        assetLoader.Setup(loader => loader.Load<PackedScene>("res://test.tscn")).ReturnsAsync(packedScene);
        Mock<ISceneNavigator> sceneNavigator = new();
        SceneAsset asset = new(assetLoader.Object, sceneNavigator.Object, "res://test.tscn");

        await asset.Open();

        sceneNavigator.Verify(navigator => navigator.ChangeScene(packedScene), Times.Once);
    }
}
