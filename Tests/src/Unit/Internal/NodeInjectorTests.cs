namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="NodeInjector"/>.
/// </summary>
public sealed class NodeInjectorTests
{
    [Fact]
    public void GetInjectableProperties_ReturnsOnlyInjectAttributedProperties()
    {
        NodeInjector injector = new(new Mock<IServiceProvider>().Object);

        PropertyInfo[] properties = injector.GetInjectableProperties(typeof(InjectableThing));

        Assert.Equal(["First", "Second"], properties.Select(property => property.Name).Order());
    }

    [Fact]
    public void GetInjectableProperties_CachesResultForSameType()
    {
        NodeInjector injector = new(new Mock<IServiceProvider>().Object);

        PropertyInfo[] first = injector.GetInjectableProperties(typeof(InjectableThing));
        PropertyInfo[] second = injector.GetInjectableProperties(typeof(InjectableThing));

        Assert.Same(first, second);
    }

    [Fact]
    public void GetInjectableProperties_Throws_WhenInjectPropertyIsNotFullyPublic()
    {
        NodeInjector injector = new(new Mock<IServiceProvider>().Object);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => injector.GetInjectableProperties(typeof(NonPublicInjectableThing)));
        Assert.Equal(
            $"'{nameof(NonPublicInjectableThing)}.{nameof(NonPublicInjectableThing.Invalid)}' is marked with [Inject] but is not a public property.",
            exception.Message);
    }

    [Fact]
    public void GetInjectableProperties_Throws_WhenInjectPropertyGetterIsNotPublic()
    {
        NodeInjector injector = new(new Mock<IServiceProvider>().Object);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => injector.GetInjectableProperties(typeof(NonPublicGetterInjectableThing)));
        Assert.Equal(
            $"'{nameof(NonPublicGetterInjectableThing)}.{nameof(NonPublicGetterInjectableThing.Invalid)}' is marked with [Inject] but is not a public property.",
            exception.Message);
    }

    [Fact]
    public void Inject_PopulatesInjectAttributedPropertiesOnANode()
    {
        object expected = new();
        Mock<IServiceProvider> provider = new();
        provider.Setup(p => p.GetService(typeof(object))).Returns(expected);
        NodeInjector injector = new(provider.Object);
        InjectableNode node = GodotObjects.CreateUninitialized<InjectableNode>();

        injector.Inject(node);

        Assert.Same(expected, node.Value);
    }

    [Fact]
    public void Inject_PopulatesKeyedInjectAttributedPropertiesOnANode_FromTheMatchingKeyedRegistration()
    {
        object expected = new();
        ServiceCollection services = new();
        services.AddKeyedSingleton("Key", expected);
        using ServiceProvider provider = services.BuildServiceProvider();
        NodeInjector injector = new(provider);
        KeyedInjectableNode node = GodotObjects.CreateUninitialized<KeyedInjectableNode>();

        injector.Inject(node);

        Assert.Same(expected, node.Value);
    }
}
