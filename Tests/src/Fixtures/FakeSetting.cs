namespace Markwardt.GodotServices.Tests;

/// <summary>
/// A minimal <see cref="ISetting{T}"/> used to test <see cref="ServiceCollectionExtensions.AddSetting{T}(IServiceCollection, object, Func{IServiceProvider, ISetting{T}})"/>.
/// </summary>
internal sealed class FakeSetting : ISetting<int>
{
    /// <inheritdoc />
    public int Get() => 0;
}
