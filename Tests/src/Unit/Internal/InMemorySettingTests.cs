namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="InMemorySetting{T}"/>.
/// </summary>
public sealed class InMemorySettingTests
{
    [Fact]
    public void Get_ReturnsTheInitialValue()
    {
        InMemorySetting<int> setting = new(5);

        Assert.Equal(5, setting.Get());
    }

    [Fact]
    public void Set_ChangesTheValueGetReturns()
    {
        InMemorySetting<int> setting = new(5);

        setting.Set(10);

        Assert.Equal(10, setting.Get());
    }
}
