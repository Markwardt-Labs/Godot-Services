namespace Markwardt.GodotServices;

/// <summary>
/// Composes the application's <see cref="IServiceProvider"/> and wires up automatic
/// <see cref="InjectAttribute"/> property injection for every node that subsequently enters the
/// scene tree. Construct one from a project's own autoload node's <c>_EnterTree</c>, and call
/// <see cref="Dispose"/> from its <c>_ExitTree</c> - or extend <see cref="ServiceManager"/>, which
/// already does so.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ServiceContainer : IDisposable
{
    private readonly SceneTree tree;
    private readonly SceneTree.NodeAddedEventHandler nodeAddedHandler;

    /// <summary>
    /// Composes the application's <see cref="IServiceProvider"/>: for every currently loaded
    /// assembly marked with <see cref="ConventionScannableAttribute"/> - including this library's
    /// own - registers that assembly's convention-matched services and node-backed singletons
    /// (see <see cref="ServiceCollectionExtensions.AddConventionServices(IServiceCollection, Assembly)"/>),
    /// registers this library's own <see cref="GameSettings"/> settings and every
    /// <see cref="Markwardt.GodotServices.ProjectSettings"/> key (each also aliased under its
    /// plain, read-only service type - see <see cref="ServiceCollectionExtensions"/>), then invokes
    /// <paramref name="configure"/> for anything else - keyed asset registrations (see
    /// <see cref="ServiceCollectionExtensions"/>), manual registrations, etc. Finally, subscribes to
    /// <paramref name="tree"/>'s <see cref="SceneTree.NodeAdded"/> to give every subsequently
    /// added node automatic <see cref="InjectAttribute"/> property injection.
    /// </summary>
    /// <param name="tree">The scene tree whose nodes get automatic <see cref="InjectAttribute"/> injection.</param>
    /// <param name="configure">An optional callback for additional service registration.</param>
    public ServiceContainer(SceneTree tree, Action<IServiceCollection>? configure = null)
    {
        this.tree = tree;

        ServiceCollection services = new();
        NodeSingletonRegistry nodeSingletons = new(services);

        foreach (Assembly assembly in ConventionScannableAssemblies.Discover())
        {
            services.AddConventionServices(assembly);
            nodeSingletons.RegisterConventionNodes(assembly);
        }

        AddSettings(services);
        AddProjectSettings(services);

        configure?.Invoke(services);

        Provider = services.BuildServiceProvider();

        NodeInjector injector = new(Provider);
        nodeAddedHandler = node =>
        {
            nodeSingletons.Capture(node);
            injector.Inject(node);
        };
        tree.NodeAdded += nodeAddedHandler;
    }

    /// <summary>
    /// The composed service provider.
    /// </summary>
    public IServiceProvider Provider { get; }

    /// <summary>
    /// Unsubscribes from <see cref="SceneTree.NodeAdded"/>.
    /// </summary>
    public void Dispose() => tree.NodeAdded -= nodeAddedHandler;

    /// <summary>
    /// Registers this library's own <see cref="GameSettings"/> settings as keyed
    /// <see cref="IMutableSetting{T}"/> singletons, each also aliased under the plain
    /// <see cref="ISetting{T}"/> service type.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    private void AddSettings(IServiceCollection services)
    {
        Window root = tree.Root;

        services
            .AddSetting<DisplayServer.WindowMode>(GameSettings.WindowMode, _ => new WindowMode())
            .AddSetting<DisplayServer.VSyncMode>(GameSettings.VSyncMode, _ => new VSyncMode())
            .AddSetting<Vector2I>(GameSettings.WindowSize, _ => new WindowSize())
            .AddSetting<int>(GameSettings.CurrentScreen, _ => new CurrentScreen())
            .AddSetting<Viewport.Msaa>(GameSettings.Msaa2D, _ => new Msaa2D(root))
            .AddSetting<Viewport.Msaa>(GameSettings.Msaa3D, _ => new Msaa3D(root))
            .AddSetting<Viewport.ScreenSpaceAAEnum>(GameSettings.ScreenSpaceAA, _ => new ScreenSpaceAA(root))
            .AddSetting<Viewport.Scaling3DModeEnum>(GameSettings.Scaling3DMode, _ => new Scaling3DMode(root))
            .AddSetting<float>(GameSettings.Scaling3DScale, _ => new Scaling3DScale(root))
            .AddSetting<float>(GameSettings.ContentScaleFactor, _ => new ContentScaleFactor(root))
            .AddSetting<string>(GameSettings.AudioOutputDevice, _ => new AudioOutputDevice())
            .AddSetting<string>(GameSettings.AudioInputDevice, _ => new AudioInputDevice())
            .AddSetting<Godot.Input.MouseModeEnum>(GameSettings.MouseMode, _ => new MouseMode())
            .AddSetting<int>(GameSettings.MaxFps, _ => new MaxFps())
            .AddSetting<string>(GameSettings.Locale, _ => new Locale());
    }

    /// <summary>
    /// Registers every <see cref="Markwardt.GodotServices.ProjectSettings"/> key as a keyed
    /// <see cref="IMutableSetting{T}"/> singleton, using each key as its own name (see
    /// <see cref="ServiceCollectionExtensions.AddProjectSetting{T}(IServiceCollection, string)"/>).
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    private void AddProjectSettings(IServiceCollection services)
    {
        services
            .AddProjectSetting<string>(ProjectSettings.Application.ConfigName)
            .AddProjectSetting<string>(ProjectSettings.Application.ConfigDescription)
            .AddProjectSetting<string>(ProjectSettings.Application.ConfigIcon)
            .AddProjectSetting<string>(ProjectSettings.Application.RunMainScene)
            .AddProjectSetting<Color>(ProjectSettings.Application.BootSplashBgColor)
            .AddProjectSetting<string>(ProjectSettings.Application.BootSplashImage)
            .AddProjectSetting<int>(ProjectSettings.Display.WindowWidth)
            .AddProjectSetting<int>(ProjectSettings.Display.WindowHeight)
            .AddProjectSetting<int>(ProjectSettings.Display.WindowMode)
            .AddProjectSetting<bool>(ProjectSettings.Display.WindowResizable)
            .AddProjectSetting<string>(ProjectSettings.Display.StretchMode)
            .AddProjectSetting<string>(ProjectSettings.Display.StretchAspect)
            .AddProjectSetting<int>(ProjectSettings.Display.VSyncMode)
            .AddProjectSetting<string>(ProjectSettings.Audio.DefaultBusLayout)
            .AddProjectSetting<int>(ProjectSettings.Audio.DefaultPlaybackType)
            .AddProjectSetting<int>(ProjectSettings.Rendering.Msaa2D)
            .AddProjectSetting<int>(ProjectSettings.Rendering.Msaa3D)
            .AddProjectSetting<int>(ProjectSettings.Rendering.ScreenSpaceAA)
            .AddProjectSetting<Color>(ProjectSettings.Rendering.DefaultClearColor)
            .AddProjectSetting<int>(ProjectSettings.Rendering.Scaling3DMode)
            .AddProjectSetting<float>(ProjectSettings.Rendering.Scaling3DScale)
            .AddProjectSetting<int>(ProjectSettings.Rendering.DirectionalShadowSize)
            .AddProjectSetting<bool>(ProjectSettings.Rendering.UseOcclusionCulling)
            .AddProjectSetting<float>(ProjectSettings.Physics.Gravity2D)
            .AddProjectSetting<Vector2>(ProjectSettings.Physics.GravityVector2D)
            .AddProjectSetting<float>(ProjectSettings.Physics.Gravity3D)
            .AddProjectSetting<Vector3>(ProjectSettings.Physics.GravityVector3D)
            .AddProjectSetting<int>(ProjectSettings.Physics.PhysicsTicksPerSecond)
            .AddProjectSetting<int>(ProjectSettings.Physics.MaxPhysicsStepsPerFrame)
            .AddProjectSetting<bool>(ProjectSettings.InputDevices.EmulateTouchFromMouse)
            .AddProjectSetting<bool>(ProjectSettings.InputDevices.EmulateMouseFromTouch)
            .AddProjectSetting<int>(ProjectSettings.Debug.ForceFps)
            .AddProjectSetting<bool>(ProjectSettings.Debug.PrintFps)
            .AddProjectSetting<bool>(ProjectSettings.Gui.SnapControlsToPixels);
    }
}
