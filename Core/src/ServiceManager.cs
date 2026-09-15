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
    /// Constructs the <see cref="ServiceContainer"/> that composes this application's services,
    /// via <see cref="Setup"/> - unless <see cref="IsTestLaunch"/> says this launch is a test run
    /// rather than the actual application, in which case <see cref="OnTestLaunch"/> runs instead,
    /// deferred to the next idle frame. Deferring matters because this method itself runs mid-way
    /// through the *entire* initial tree (every autoload plus the main scene) entering the scene
    /// tree, before any of it reaches <c>_Ready</c> - <see cref="OnTestLaunch"/> typically needs to
    /// freely add and free nodes (a test runner setting up and tearing down each test's fixtures,
    /// say), which isn't safe to do synchronously while the engine is still walking that same tree
    /// to finish entering it.
    /// </summary>
    public override void _EnterTree()
    {
        if (IsTestLaunch())
        {
            CallDeferred(nameof(OnTestLaunch));
            return;
        }

        Setup();
    }

    /// <summary>
    /// Disposes the <see cref="ServiceContainer"/> constructed by <see cref="Setup"/>.
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

    /// <summary>
    /// Gets whether <see cref="_EnterTree"/> should skip building the <see cref="ServiceContainer"/>
    /// in favor of <see cref="OnTestLaunch"/> - for a launch that's actually a test run rather than
    /// the real application, like one triggered by a command-line test runner. Returns
    /// <see langword="false"/> by default, always proceeding with normal setup; override to
    /// recognize a project's own test-launch condition, e.g. by inspecting
    /// <c>OS.GetCmdlineArgs()</c> for a test runner's own flag.
    /// </summary>
    /// <returns>Whether this launch is a test run, so <see cref="Setup"/> should be skipped.</returns>
    protected virtual bool IsTestLaunch() => false;

    /// <summary>
    /// Runs instead of <see cref="Setup"/> once <see cref="IsTestLaunch"/> returns
    /// <see langword="true"/>, deferred to the next idle frame (see <see cref="_EnterTree"/>) so
    /// it's safe to mutate the scene tree from it. Does nothing by default; override to run
    /// whatever the test launch actually needs to do instead, e.g. handing off to a test
    /// framework's own runner.
    /// </summary>
    protected virtual void OnTestLaunch()
    {
    }

    /// <summary>
    /// Builds the application's <see cref="IServiceProvider"/> and wires up automatic
    /// <see cref="InjectAttribute"/> property injection for every node that subsequently enters
    /// the scene tree, by constructing a <see cref="ServiceContainer"/>. Exposed under its own
    /// name rather than folded directly into <see cref="_EnterTree"/>, so a project whose own
    /// <see cref="IsTestLaunch"/> override always returns <see langword="true"/> during a test
    /// session can still call this directly, to exercise the real, convention-built container end
    /// to end from a test.
    /// </summary>
    protected void Setup() => container = new ServiceContainer(GetTree(), Configure);
}
