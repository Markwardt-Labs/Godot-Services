namespace Markwardt.GodotServices;

/// <summary>
/// Extension methods for registering services with <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers every concrete class in <paramref name="assembly"/> that implements an
    /// interface named after itself (e.g. <c>Thing</c> implementing <c>IThing</c>) as a
    /// singleton, without requiring explicit registration. A type annotated with
    /// <see cref="TransientAttribute"/> is registered as transient instead. <see cref="Node"/>
    /// subclasses are skipped, since those are registered as node-backed singletons by
    /// <see cref="ServiceContainer"/> instead.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="assembly">The assembly to scan for conventionally-named services.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddConventionServices(this IServiceCollection services, Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition || typeof(Node).IsAssignableFrom(type))
            {
                continue;
            }

            Type? contract = type.GetInterface($"I{type.Name}");

            if (contract is not null)
            {
                if (type.IsDefined(typeof(TransientAttribute)))
                {
                    services.AddTransient(contract, type);
                }
                else
                {
                    services.AddSingleton(contract, type);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Registers convention-matched services (see the other
    /// <see cref="AddConventionServices(IServiceCollection, Assembly)"/> overload) from every
    /// currently loaded assembly marked with
    /// <see cref="ConventionScannableAttribute"/>, including this library's own.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddConventionServices(this IServiceCollection services)
    {
        foreach (Assembly assembly in ConventionScannableAssemblies.Discover())
        {
            services.AddConventionServices(assembly);
        }

        return services;
    }

    /// <summary>
    /// Registers a custom asset adapter built by <paramref name="factory"/> as a keyed
    /// <see cref="IAsset{T}"/> singleton under <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type this asset loads as.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the asset under.</param>
    /// <param name="factory">Builds the asset adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddAsset<T>(this IServiceCollection services, object key, Func<IServiceProvider, IAsset<T>> factory)
        => services.AddKeyedSingleton<IAsset<T>>(key, (provider, _) => factory(provider));

    /// <summary>
    /// Registers a <see cref="ResourceAsset{TResource}"/> for the resource at <paramref name="path"/>
    /// as a keyed <see cref="IAsset{T}"/> singleton.
    /// </summary>
    /// <typeparam name="TResource">The resource type the asset loads as.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the asset under.</param>
    /// <param name="path">The <c>res://</c> path to the resource.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddAsset<TResource>(this IServiceCollection services, object key, string path)
        where TResource : Resource
        => services.AddAsset<TResource>(key, provider => new ResourceAsset<TResource>(provider.GetRequiredService<IAssetLoader>(), path));

    /// <summary>
    /// Registers a custom <see cref="ISceneAsset"/> built by <paramref name="factory"/> as a
    /// keyed singleton under <paramref name="key"/>, also aliasing the same instance under the
    /// plain <see cref="IAsset{T}"/> (closed over <see cref="PackedScene"/>) service type, for a
    /// call site that only needs to load the scene rather than instantiate or open it.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the asset under.</param>
    /// <param name="factory">Builds the scene asset adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddScene(this IServiceCollection services, object key, Func<IServiceProvider, ISceneAsset> factory)
        => services
            .AddKeyedSingleton<ISceneAsset>(key, (provider, _) => factory(provider))
            .AddKeyedAlias<IAsset<PackedScene>, ISceneAsset>(key);

    /// <summary>
    /// Registers a <see cref="SceneAsset"/> for the packed scene at <paramref name="path"/> as a
    /// keyed <see cref="ISceneAsset"/> singleton, also aliasing the same instance under the plain
    /// <see cref="IAsset{T}"/> (closed over <see cref="PackedScene"/>) service type, for a call
    /// site that only needs to load the scene rather than instantiate or open it.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the asset under.</param>
    /// <param name="path">The <c>res://</c> path to the packed scene.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddScene(this IServiceCollection services, object key, string path)
        => services.AddScene(key, provider => new SceneAsset(
            provider.GetRequiredService<IAssetLoader>(), provider.GetRequiredService<ISceneNavigator>(), path));

    /// <summary>
    /// Registers a custom <see cref="IInput"/> built by <paramref name="factory"/> as a keyed
    /// singleton under <paramref name="key"/>.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input action under.</param>
    /// <param name="factory">Builds the read-only input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInput(this IServiceCollection services, object key, Func<IServiceProvider, IInput> factory)
        => services.AddKeyedSingleton<IInput>(key, (provider, _) => factory(provider));

    /// <summary>
    /// Registers a custom <see cref="IRebindableInput"/> built by <paramref name="factory"/> as a
    /// keyed singleton under <paramref name="key"/>, also aliasing the same instance under the
    /// plain <see cref="IInput"/> service type, for a call site that only reads the action.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input action under.</param>
    /// <param name="factory">Builds the rebindable input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInput(this IServiceCollection services, object key, Func<IServiceProvider, IRebindableInput> factory)
        => services
            .AddKeyedSingleton<IRebindableInput>(key, (provider, _) => factory(provider))
            .AddKeyedAlias<IInput, IRebindableInput>(key);

    /// <summary>
    /// Registers an <see cref="Internal.Input"/> bound to <paramref name="action"/> as a keyed
    /// <see cref="IRebindableInput"/> singleton, also aliasing the same instance under the plain
    /// <see cref="IInput"/> service type, for a call site that only reads the action.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input action under.</param>
    /// <param name="action">The name of the input action to bind to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInput(this IServiceCollection services, object key, string action)
        => services.AddInput(key, _ => new Internal.Input(action));

    /// <summary>
    /// Registers a custom <see cref="IInputVector"/> built by <paramref name="factory"/> as a
    /// keyed singleton under <paramref name="key"/>.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input vector under.</param>
    /// <param name="factory">Builds the read-only input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputVector(this IServiceCollection services, object key, Func<IServiceProvider, IInputVector> factory)
        => services.AddKeyedSingleton<IInputVector>(key, (provider, _) => factory(provider));

    /// <summary>
    /// Registers a custom <see cref="IRebindableInputVector"/> built by <paramref name="factory"/>
    /// as a keyed singleton under <paramref name="key"/>, also aliasing the same instance under
    /// the plain <see cref="IInputVector"/> service type, for a call site that only reads the vector.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input vector under.</param>
    /// <param name="factory">Builds the rebindable input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputVector(this IServiceCollection services, object key, Func<IServiceProvider, IRebindableInputVector> factory)
        => services
            .AddKeyedSingleton<IRebindableInputVector>(key, (provider, _) => factory(provider))
            .AddKeyedAlias<IInputVector, IRebindableInputVector>(key);

    /// <summary>
    /// Registers an <see cref="InputVector"/> bound to <paramref name="negativeX"/>/
    /// <paramref name="positiveX"/>/<paramref name="negativeY"/>/<paramref name="positiveY"/> as a
    /// keyed <see cref="IRebindableInputVector"/> singleton, also aliasing the same instance under
    /// the plain <see cref="IInputVector"/> service type, for a call site that only reads the vector.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input vector under.</param>
    /// <param name="negativeX">The action for negative movement along the X axis.</param>
    /// <param name="positiveX">The action for positive movement along the X axis.</param>
    /// <param name="negativeY">The action for negative movement along the Y axis.</param>
    /// <param name="positiveY">The action for positive movement along the Y axis.</param>
    /// <param name="deadzone">
    /// The deadzone to apply, or a negative value to use the average of the four actions' own deadzones.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputVector(
        this IServiceCollection services, object key, string negativeX, string positiveX, string negativeY, string positiveY, float deadzone = -1f)
        => services.AddInputVector(key, _ => new InputVector(negativeX, positiveX, negativeY, positiveY, deadzone));

    /// <summary>
    /// Registers a custom <see cref="IInputAxis"/> built by <paramref name="factory"/> as a keyed
    /// singleton under <paramref name="key"/>.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input axis under.</param>
    /// <param name="factory">Builds the read-only input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputAxis(this IServiceCollection services, object key, Func<IServiceProvider, IInputAxis> factory)
        => services.AddKeyedSingleton<IInputAxis>(key, (provider, _) => factory(provider));

    /// <summary>
    /// Registers a custom <see cref="IRebindableInputAxis"/> built by <paramref name="factory"/>
    /// as a keyed singleton under <paramref name="key"/>, also aliasing the same instance under
    /// the plain <see cref="IInputAxis"/> service type, for a call site that only reads the axis.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input axis under.</param>
    /// <param name="factory">Builds the rebindable input adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputAxis(this IServiceCollection services, object key, Func<IServiceProvider, IRebindableInputAxis> factory)
        => services
            .AddKeyedSingleton<IRebindableInputAxis>(key, (provider, _) => factory(provider))
            .AddKeyedAlias<IInputAxis, IRebindableInputAxis>(key);

    /// <summary>
    /// Registers an <see cref="InputAxis"/> bound to <paramref name="negativeAction"/>/
    /// <paramref name="positiveAction"/> as a keyed <see cref="IRebindableInputAxis"/> singleton,
    /// also aliasing the same instance under the plain <see cref="IInputAxis"/> service type, for
    /// a call site that only reads the axis.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the input axis under.</param>
    /// <param name="negativeAction">The action for the negative direction.</param>
    /// <param name="positiveAction">The action for the positive direction.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddInputAxis(this IServiceCollection services, object key, string negativeAction, string positiveAction)
        => services.AddInputAxis(key, _ => new InputAxis(negativeAction, positiveAction));

    /// <summary>
    /// Registers a <see cref="ProjectSetting{T}"/> bound to <paramref name="name"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, also aliasing the same instance under the plain
    /// <see cref="ISetting{T}"/> service type, for a call site that only reads the setting.
    /// </summary>
    /// <typeparam name="T">The type of the setting's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="name">The name of the project setting to bind to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddProjectSetting<[MustBeVariant] T>(this IServiceCollection services, object key, string name)
        => services.AddSetting<T>(key, _ => new ProjectSetting<T>(name));

    /// <summary>
    /// Registers a <see cref="ProjectSetting{T}"/> bound to <paramref name="name"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, using <paramref name="name"/> as both the
    /// project setting to bind to and the key to register it under - convenient for a
    /// well-known, already-unique path from <see cref="Markwardt.GodotServices.ProjectSettings"/>,
    /// which otherwise has no separate key of its own to supply.
    /// </summary>
    /// <typeparam name="T">The type of the setting's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="name">The name of the project setting to bind to, and the key to register it under.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddProjectSetting<[MustBeVariant] T>(this IServiceCollection services, string name)
        => services.AddProjectSetting<T>(name, name);

    /// <summary>
    /// Registers a <see cref="ShaderGlobal{T}"/> bound to <paramref name="name"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, also aliasing the same instance under the
    /// plain <see cref="ISetting{T}"/> service type, for a call site that only reads the setting.
    /// </summary>
    /// <typeparam name="T">The type of the shader global's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="name">The name of the global shader parameter to bind to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddShaderSetting<[MustBeVariant] T>(this IServiceCollection services, object key, string name)
        => services.AddSetting<T>(key, _ => new ShaderGlobal<T>(name));

    /// <summary>
    /// Registers an <see cref="AudioVolume"/> bound to <paramref name="bus"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, also aliasing the same instance under the
    /// plain <see cref="ISetting{T}"/> service type, for a call site that only reads the setting.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="bus">The name of the audio bus to bind to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddVolumeSetting(this IServiceCollection services, object key, string bus)
        => services.AddSetting<float>(key, _ => new AudioVolume(bus));

    /// <summary>
    /// Registers an <see cref="AudioMute"/> bound to <paramref name="bus"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, also aliasing the same instance under the
    /// plain <see cref="ISetting{T}"/> service type, for a call site that only reads the setting.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="bus">The name of the audio bus to bind to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddMuteSetting(this IServiceCollection services, object key, string bus)
        => services.AddSetting<bool>(key, _ => new AudioMute(bus));

    /// <summary>
    /// Registers a custom setting adapter built by <paramref name="factory"/> as a keyed
    /// <see cref="ISetting{T}"/> singleton under <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the setting's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="factory">Builds the read-only setting adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddSetting<T>(this IServiceCollection services, object key, Func<IServiceProvider, ISetting<T>> factory)
        => services.AddKeyedSingleton<ISetting<T>>(key, (provider, _) => factory(provider));

    /// <summary>
    /// Registers a custom setting adapter built by <paramref name="factory"/> as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton under <paramref name="key"/>, also aliasing the
    /// same instance under the plain <see cref="ISetting{T}"/> service type.
    /// </summary>
    /// <typeparam name="T">The type of the setting's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="factory">Builds the mutable setting adapter to register, given the composed <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddSetting<T>(this IServiceCollection services, object key, Func<IServiceProvider, IMutableSetting<T>> factory)
        => services
            .AddKeyedSingleton<IMutableSetting<T>>(key, (provider, _) => factory(provider))
            .AddKeyedAlias<ISetting<T>, IMutableSetting<T>>(key);

    /// <summary>
    /// Registers an <see cref="InMemorySetting{T}"/>, starting at <paramref name="initialValue"/>,
    /// as a keyed <see cref="IMutableSetting{T}"/> singleton under <paramref name="key"/>, also
    /// aliasing the same instance under the plain <see cref="ISetting{T}"/> service type.
    /// </summary>
    /// <typeparam name="T">The type of the setting's value.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key to register the setting under.</param>
    /// <param name="initialValue">The setting's initial value.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddSetting<T>(this IServiceCollection services, object key, T initialValue)
        => services.AddSetting<T>(key, _ => new InMemorySetting<T>(initialValue));

    /// <summary>
    /// Registers <typeparamref name="TDerived"/>'s existing keyed singleton registration under
    /// <paramref name="key"/> again, under its base <typeparamref name="TBase"/> service type,
    /// resolving to the same singleton instance.
    /// </summary>
    /// <typeparam name="TBase">The base service type to alias under.</typeparam>
    /// <typeparam name="TDerived">The already-registered derived service type.</typeparam>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="key">The key <typeparamref name="TDerived"/> is already registered under.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    private static IServiceCollection AddKeyedAlias<TBase, TDerived>(this IServiceCollection services, object key)
        where TBase : class
        where TDerived : class, TBase
        => services.AddKeyedSingleton<TBase>(key, (provider, _) => provider.GetRequiredKeyedService<TDerived>(key));
}
