namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="ResourceAsset{TResource}"/>.
/// </summary>
public sealed class ResourceAssetTests
{
    [Fact]
    public async Task Load_ReturnsTheResourceLoadedFromThePath()
    {
        Resource expected = GodotObjects.CreateUninitialized<Resource>();
        Mock<IAssetLoader> assetLoader = new();
        assetLoader.Setup(loader => loader.Load<Resource>("res://test.tres")).ReturnsAsync(expected);
        ResourceAsset<Resource> asset = new(assetLoader.Object, "res://test.tres");

        Resource actual = await asset.Load();

        Assert.Same(expected, actual);
    }
}
