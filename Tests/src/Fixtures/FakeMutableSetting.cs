namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="IMutableSetting{T}"/> used to test <see cref="ServiceCollectionExtensions.AddSetting{T}(IServiceCollection, object, Func{IServiceProvider, IMutableSetting{T}})"/>.
/// </summary>
internal sealed class FakeMutableSetting : IMutableSetting<int>
{
    /// <inheritdoc />
    public int Get() => 0;

    /// <inheritdoc />
    public void Set(int value)
    {
    }
}
