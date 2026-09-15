namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="NodeSingletonRegistry"/>.
/// </summary>
public sealed class NodeSingletonRegistryTests
{
    [Fact]
    public void Register_AddsSingletonFactory()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);

        registry.Register<IConventionNode, ConventionNode>();

        ServiceDescriptor[] matches = [.. services.Where(descriptor => descriptor.ServiceType == typeof(IConventionNode))];
        Assert.Single(matches);
        Assert.Equal(ServiceLifetime.Singleton, matches[0].Lifetime);
    }

    [Fact]
    public void Register_TransientAttributeNode_AddsTransientFactory()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);

        registry.Register<ITransientConventionNode, TransientConventionNode>();

        ServiceDescriptor[] matches = [.. services.Where(descriptor => descriptor.ServiceType == typeof(ITransientConventionNode))];
        Assert.Single(matches);
        Assert.Equal(ServiceLifetime.Transient, matches[0].Lifetime);
    }

    [Fact]
    public void Resolving_BeforeNodeIsCaptured_Throws()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);
        registry.Register<IConventionNode, ConventionNode>();

        using ServiceProvider provider = services.BuildServiceProvider();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IConventionNode>());
        Assert.Equal($"'{nameof(ConventionNode)}' has not entered the scene tree yet.", exception.Message);
    }

    [Fact]
    public void RegisterConventionNodes_RegistersNodeSubclassesMatchingNamingConvention()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);

        registry.RegisterConventionNodes(typeof(ConventionNode).Assembly);

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IConventionNode) && descriptor.Lifetime == ServiceLifetime.Singleton);
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(ITransientConventionNode) && descriptor.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void RegisterConventionNodes_SkipsNonNodeConventionTypes()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);

        registry.RegisterConventionNodes(typeof(ConventionThing).Assembly);

        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IConventionThing));
    }

    [Fact]
    public void Capture_ThenResolve_ReturnsTheCapturedNode()
    {
        ServiceCollection services = new();
        NodeSingletonRegistry registry = new(services);
        registry.Register<IConventionNode, ConventionNode>();
        ConventionNode node = GodotObjects.CreateUninitialized<ConventionNode>();

        registry.Capture(node);

        using ServiceProvider provider = services.BuildServiceProvider();
        Assert.Same(node, provider.GetRequiredService<IConventionNode>());
    }
}
