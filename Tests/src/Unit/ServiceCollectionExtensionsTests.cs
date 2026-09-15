namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddConventionServices_RegistersConventionMatchAsSingleton()
    {
        ServiceCollection services = new();

        services.AddConventionServices(typeof(ConventionThing).Assembly);

        ServiceDescriptor[] matches = [.. services.Where(descriptor => descriptor.ServiceType == typeof(IConventionThing))];
        Assert.Single(matches);
        Assert.Equal(typeof(ConventionThing), matches[0].ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, matches[0].Lifetime);
    }

    [Fact]
    public void AddConventionServices_RegistersTransientAttributeMatchAsTransient()
    {
        ServiceCollection services = new();

        services.AddConventionServices(typeof(TransientConventionThing).Assembly);

        ServiceDescriptor[] matches = [.. services.Where(descriptor => descriptor.ServiceType == typeof(ITransientConventionThing))];
        Assert.Single(matches);
        Assert.Equal(typeof(TransientConventionThing), matches[0].ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, matches[0].Lifetime);
    }

    [Fact]
    public void AddConventionServices_SkipsNodeSubclasses()
    {
        ServiceCollection services = new();

        services.AddConventionServices(typeof(ConventionNode).Assembly);

        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IConventionNode));
    }

    [Fact]
    public void AddConventionServices_NoArgument_ScansEveryConventionScannableAssembly()
    {
        ServiceCollection services = new();

        services.AddConventionServices();

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IAssetLoader));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ISceneNavigator));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IRaycaster));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IClock));
    }

    [Fact]
    public void AddAsset_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddAsset<Resource>("Key", "res://test.tres");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IAsset<Resource>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddAsset_WithFactory_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddAsset<int>("Key", _ => new FakeAsset());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IAsset<int>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddScene_WithFactory_RegistersKeyedSingletonAndIAssetAlias()
    {
        ServiceCollection services = new();

        services.AddScene("Key", _ => new FakeSceneAsset());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISceneAsset));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceDescriptor aliasDescriptor = Assert.Single(services, d => d.ServiceType == typeof(IAsset<PackedScene>));
        Assert.Equal(ServiceLifetime.Singleton, aliasDescriptor.Lifetime);
        Assert.True(aliasDescriptor.IsKeyedService);
        Assert.Equal("Key", aliasDescriptor.ServiceKey);
    }

    [Fact]
    public void AddScene_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddScene("Key", "res://test.tscn");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISceneAsset));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddScene_AlsoRegistersKeyedIAssetAliasOfSameInstance()
    {
        ServiceCollection services = new();
        services.AddSingleton(Mock.Of<IAssetLoader>());
        services.AddSingleton(Mock.Of<ISceneNavigator>());

        services.AddScene("Key", "res://test.tscn");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IAsset<PackedScene>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceProvider provider = services.BuildServiceProvider();
        Assert.Same(
            provider.GetRequiredKeyedService<ISceneAsset>("Key"),
            provider.GetRequiredKeyedService<IAsset<PackedScene>>("Key"));
    }

    [Fact]
    public void AddInput_WithIInputFactory_RegistersKeyedSingletonAndNoRebindableAlias()
    {
        ServiceCollection services = new();

        services.AddInput("Key", _ => new FakeInput());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInput));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IRebindableInput));
    }

    [Fact]
    public void AddInput_WithFactory_RegistersKeyedSingletonAndIInputAliasOfSameInstance()
    {
        ServiceCollection services = new();

        services.AddInput("Key", _ => new FakeRebindableInput());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInput));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceDescriptor aliasDescriptor = Assert.Single(services, d => d.ServiceType == typeof(IInput));
        Assert.Equal(ServiceLifetime.Singleton, aliasDescriptor.Lifetime);
        Assert.True(aliasDescriptor.IsKeyedService);
        Assert.Equal("Key", aliasDescriptor.ServiceKey);

        ServiceProvider provider = services.BuildServiceProvider();
        Assert.Same(provider.GetRequiredKeyedService<IRebindableInput>("Key"), provider.GetRequiredKeyedService<IInput>("Key"));
    }

    [Fact]
    public void AddInput_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddInput("Key", "jump");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInput));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddInput_AlsoRegistersKeyedIInputAlias()
    {
        ServiceCollection services = new();

        services.AddInput("Key", "jump");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInput));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddInputVector_WithIInputVectorFactory_RegistersKeyedSingletonAndNoRebindableAlias()
    {
        ServiceCollection services = new();

        services.AddInputVector("Key", _ => new FakeInputVector());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputVector));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IRebindableInputVector));
    }

    [Fact]
    public void AddInputVector_WithFactory_RegistersKeyedSingletonAndIInputVectorAlias()
    {
        ServiceCollection services = new();

        services.AddInputVector("Key", _ => new FakeRebindableInputVector());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInputVector));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceDescriptor aliasDescriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputVector));
        Assert.Equal(ServiceLifetime.Singleton, aliasDescriptor.Lifetime);
        Assert.True(aliasDescriptor.IsKeyedService);
        Assert.Equal("Key", aliasDescriptor.ServiceKey);
    }

    [Fact]
    public void AddInputVector_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddInputVector("Key", "left", "right", "up", "down");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInputVector));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddInputVector_AlsoRegistersKeyedIInputVectorAlias()
    {
        ServiceCollection services = new();

        services.AddInputVector("Key", "left", "right", "up", "down");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputVector));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddInputAxis_WithIInputAxisFactory_RegistersKeyedSingletonAndNoRebindableAlias()
    {
        ServiceCollection services = new();

        services.AddInputAxis("Key", _ => new FakeInputAxis());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputAxis));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IRebindableInputAxis));
    }

    [Fact]
    public void AddInputAxis_WithFactory_RegistersKeyedSingletonAndIInputAxisAlias()
    {
        ServiceCollection services = new();

        services.AddInputAxis("Key", _ => new FakeRebindableInputAxis());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInputAxis));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceDescriptor aliasDescriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputAxis));
        Assert.Equal(ServiceLifetime.Singleton, aliasDescriptor.Lifetime);
        Assert.True(aliasDescriptor.IsKeyedService);
        Assert.Equal("Key", aliasDescriptor.ServiceKey);
    }

    [Fact]
    public void AddInputAxis_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddInputAxis("Key", "zoom_out", "zoom_in");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IRebindableInputAxis));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddInputAxis_AlsoRegistersKeyedIInputAxisAlias()
    {
        ServiceCollection services = new();

        services.AddInputAxis("Key", "zoom_out", "zoom_in");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IInputAxis));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddProjectSetting_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddProjectSetting<float>("Key", "audio/master_volume");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddProjectSetting_AlsoRegistersKeyedISettingAliasOfSameInstance()
    {
        ServiceCollection services = new();

        services.AddProjectSetting<float>("Key", "audio/master_volume");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceProvider provider = services.BuildServiceProvider();
        Assert.Same(provider.GetRequiredKeyedService<IMutableSetting<float>>("Key"), provider.GetRequiredKeyedService<ISetting<float>>("Key"));
    }

    [Fact]
    public void AddSetting_WithISettingFactory_RegistersKeyedSingletonAndNoMutableAlias()
    {
        ServiceCollection services = new();

        services.AddSetting<int>("Key", _ => new FakeSetting());

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<int>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IMutableSetting<int>));
    }

    [Fact]
    public void AddSetting_WithIMutableSettingFactory_RegistersKeyedSingletonAndISettingAliasOfSameInstance()
    {
        ServiceCollection services = new();

        services.AddSetting<int>("Key", _ => new FakeMutableSetting());

        ServiceDescriptor mutableDescriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<int>));
        Assert.Equal(ServiceLifetime.Singleton, mutableDescriptor.Lifetime);
        Assert.True(mutableDescriptor.IsKeyedService);
        Assert.Equal("Key", mutableDescriptor.ServiceKey);

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<int>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceProvider provider = services.BuildServiceProvider();
        Assert.Same(provider.GetRequiredKeyedService<IMutableSetting<int>>("Key"), provider.GetRequiredKeyedService<ISetting<int>>("Key"));
    }

    [Fact]
    public void AddSetting_WithInitialValue_RegistersKeyedSingletonStartingAtThatValue()
    {
        ServiceCollection services = new();

        services.AddSetting("Key", 5);

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<int>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);

        ServiceProvider provider = services.BuildServiceProvider();
        Assert.Equal(5, provider.GetRequiredKeyedService<IMutableSetting<int>>("Key").Get());
        Assert.Same(provider.GetRequiredKeyedService<IMutableSetting<int>>("Key"), provider.GetRequiredKeyedService<ISetting<int>>("Key"));
    }

    [Fact]
    public void AddShaderSetting_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddShaderSetting<float>("Key", "fog_density");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddShaderSetting_AlsoRegistersKeyedISettingAlias()
    {
        ServiceCollection services = new();

        services.AddShaderSetting<float>("Key", "fog_density");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddVolumeSetting_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddVolumeSetting("Key", "Music");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddVolumeSetting_AlsoRegistersKeyedISettingAlias()
    {
        ServiceCollection services = new();

        services.AddVolumeSetting("Key", "Music");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<float>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddMuteSetting_RegistersKeyedSingleton()
    {
        ServiceCollection services = new();

        services.AddMuteSetting("Key", "Music");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(IMutableSetting<bool>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }

    [Fact]
    public void AddMuteSetting_AlsoRegistersKeyedISettingAlias()
    {
        ServiceCollection services = new();

        services.AddMuteSetting("Key", "Music");

        ServiceDescriptor descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISetting<bool>));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.True(descriptor.IsKeyedService);
        Assert.Equal("Key", descriptor.ServiceKey);
    }
}
