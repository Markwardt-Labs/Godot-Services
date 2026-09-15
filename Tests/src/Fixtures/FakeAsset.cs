namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="IAsset{T}"/> used to test <see cref="ServiceCollectionExtensions.AddAsset{T}(IServiceCollection, object, Func{IServiceProvider, IAsset{T}})"/>.
/// </summary>
internal sealed class FakeAsset : IAsset<int>
{
    /// <inheritdoc />
    public Task<int> Load() => Task.FromResult(0);
}
