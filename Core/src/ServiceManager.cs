namespace Markwardt.GodotServices;

/// <summary>
/// Autoload base that composes the application's services via <see cref="ServiceContainer"/> and
/// gives every node that subsequently enters the scene tree automatic
/// <see cref="InjectAttribute"/> property injection. Extend this from a project's own autoload
/// node, and override <see cref="Configure"/> for anything convention scanning can't cover - keyed
/// asset/input registrations, manual registrations, etc.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class ServiceManager : Node
{
    private ServiceContainer? container;

    /// <summary>
    /// The composed service provider.
    /// </summary>
    /// <exception cref="InvalidOperationException">This node has not entered the scene tree yet.</exception>
    public IServiceProvider Provider =>
        container?.Provider ?? throw new InvalidOperationException($"'{GetType().Name}' has not entered the scene tree yet.");

    /// <summary>
    /// Constructs the <see cref="ServiceContainer"/> that composes this application's services.
    /// </summary>
    public override void _EnterTree() => container = new ServiceContainer(GetTree(), Configure);

    /// <summary>
    /// Disposes the <see cref="ServiceContainer"/> constructed by <see cref="_EnterTree"/>.
    /// </summary>
    public override void _ExitTree()
    {
        container?.Dispose();
        container = null;
    }

    /// <summary>
    /// Registers anything convention-based scanning can't cover - keyed asset/input
    /// registrations, manual registrations, etc. Does nothing by default; override in a derived
    /// autoload to customize service registration.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    protected virtual void Configure(IServiceCollection services)
    {
    }
}
