# Usage

Runnable examples of `ServiceManager` and the services built on top of it, in different
situations.

## Minimal setup

Mark the project's own assembly `[ConventionScannable]`, then set a script extending
`ServiceManager` on an autoload node - `ServiceManager` handles `_EnterTree`/`_ExitTree` itself:

```csharp
[assembly: ConventionScannable]

public partial class GameServices : ServiceManager;
```

Any convention-named service in the project's own assembly - a class named `Thing` implementing
`IThing` - is now registered automatically, with no further setup.

## Resolving a service into a node

```csharp
public interface IScoreKeeper
{
    void AddPoints(int amount);
}

internal sealed class ScoreKeeper : IScoreKeeper
{
    public void AddPoints(int amount) { /* ... */ }
}

public partial class Player : CharacterBody2D
{
    [Inject]
    public required IScoreKeeper ScoreKeeper { get; init; }

    private void OnCoinCollected() => ScoreKeeper.AddPoints(10);
}
```

`ScoreKeeper` is picked up by convention (`ScoreKeeper` implements `IScoreKeeper`) and registered
as a singleton; `Player.ScoreKeeper` is populated automatically as soon as the node enters the
tree.

## A service backed by an autoloaded node

A UI overlay that needs to live in the scene tree can be a node-backed singleton instead of a
plain class - the same naming convention applies:

```csharp
public interface IHud
{
    void ShowMessage(string text);
}

public partial class Hud : CanvasLayer, IHud
{
    public void ShowMessage(string text) { /* ... */ }
}
```

Any other node can now declare `[Inject] public required IHud Hud { get; init; }` and receive the
same `Hud` autoload instance, resolved once it has entered the tree.

## Loading assets by key

Register each asset once, in an override of `ServiceManager.Configure`, under a key a consuming
project declares itself:

```csharp
internal static class TextureKeys
{
    internal const string PlayerTemplate = "PlayerTemplate";
}

internal static class SceneKeys
{
    internal const string MainMenu = "MainMenu";
}

public partial class GameServices : ServiceManager
{
    protected override void Configure(IServiceCollection services)
    {
        services.AddAsset<Texture2D>(TextureKeys.PlayerTemplate, "res://Assets/Sprites/Player.png");
        services.AddScene(SceneKeys.MainMenu, "res://Scenes/MainMenu.tscn");
    }
}
```

Then resolve one by key with `[Inject]`'s key argument:

```csharp
public partial class Player : CharacterBody2D
{
    [Inject(TextureKeys.PlayerTemplate)]
    public required IAsset<Texture2D> PlayerTemplate { get; init; }

    public override async void _Ready() => GetNode<Sprite2D>("Sprite2D").Texture = await PlayerTemplate.Load();
}
```

A packed scene resolves the same way, through `ISceneAsset`, which adds `Instantiate<TNode>()` and
`Open()`:

```csharp
public partial class MainMenu : Control
{
    [Inject(SceneKeys.MainMenu)]
    public required ISceneAsset MainMenuScene { get; init; }

    private async void OnPlayAgainPressed() => await MainMenuScene.Open();
}
```

`AddScene` also registers the same instance under the plain `IAsset<PackedScene>` for the same
key, for a call site that only needs to load the scene, not instantiate or open it.

## Reading and rebinding input by key

Register each input action or combination once, in an override of `ServiceManager.Configure`,
under a key a consuming project declares itself - the same shape as registering an asset. Each
registration is rebindable from the start, so there's nothing extra to opt into later for a
settings screen that lets the player remap controls:

```csharp
internal static class ActionKeys
{
    internal const string Jump = "Jump";
    internal const string Move = "Move";
    internal const string Zoom = "Zoom";
}

public partial class GameServices : ServiceManager
{
    protected override void Configure(IServiceCollection services)
    {
        services.AddInput(ActionKeys.Jump, "jump");
        services.AddInputVector(ActionKeys.Move, "move_left", "move_right", "move_up", "move_down");
        services.AddInputAxis(ActionKeys.Zoom, "zoom_out", "zoom_in");
    }
}
```

Then resolve one by key with `[Inject]`'s key argument - the plain interface
(`IInput`/`IInputVector`/`IInputAxis`) for a call site that only ever reads the value, or the
rebindable interface (`IRebindableInput`/`IRebindableInputVector`/`IRebindableInputAxis`) for one
that also needs to remap it, both resolving the same underlying registration:

```csharp
public partial class Player : CharacterBody2D
{
    [Inject(ActionKeys.Jump)]
    public required IInput Jump { get; init; }

    [Inject(ActionKeys.Move)]
    public required IInputVector Move { get; init; }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = Move.Value * Speed;
        if (Jump.IsJustPressed())
        {
            Velocity += Vector2.Up * JumpImpulse;
        }
    }
}

public partial class ControlsMenu : Control
{
    [Inject(ActionKeys.Jump)]
    public required IRebindableInput Jump { get; init; }

    // Called once the menu has captured the player's next key press as newBinding.
    private void OnJumpKeyCaptured(InputEvent newBinding) => Jump.Rebind(newBinding);
}
```

`IRebindableInputVector`/`IRebindableInputAxis` rebind the same way, with one `Rebind*` method per
underlying action (`RebindNegativeX`/`RebindPositiveX`/`RebindNegativeY`/`RebindPositiveY` for a
vector; `RebindNegative`/`RebindPositive` for an axis), since each can be rebound independently.

## Reading and writing settings by key

Register a project setting, a global shader parameter, or an audio bus's volume/mute state once,
in an override of `ServiceManager.Configure`, under a key a consuming project declares itself -
each registers under both `IMutableSetting<T>` and the plain `ISetting<T>`:

```csharp
internal static class SettingKeys
{
    internal const string MasterVolume = "MasterVolume";
    internal const string FogColor = "FogColor";
    internal const string MusicVolume = "MusicVolume";
    internal const string MusicMuted = "MusicMuted";
}

public partial class GameServices : ServiceManager
{
    protected override void Configure(IServiceCollection services)
    {
        services.AddProjectSetting<float>(SettingKeys.MasterVolume, "audio/master_volume");
        services.AddShaderSetting<Color>(SettingKeys.FogColor, "fog_color");
        services.AddVolumeSetting(SettingKeys.MusicVolume, "Music");
        services.AddMuteSetting(SettingKeys.MusicMuted, "Music");
    }
}
```

Then resolve one by key with `[Inject]`'s key argument - the plain `ISetting<T>` for a call site
that only ever reads it, or `IMutableSetting<T>` for one that also writes it - and use it like any
other property. This library's own fixed, singular settings need no registration call at all -
`ServiceManager` makes them available automatically, under `GameSettings`' well-known keys, and
the same for every path in `ProjectSettings`, its curated list of commonly-configured project
settings - each path already doubles as its own key:

```csharp
public partial class OptionsMenu : Control
{
    [Inject(SettingKeys.MasterVolume)]
    public required IMutableSetting<float> MasterVolume { get; init; }

    [Inject(GameSettings.WindowMode)]
    public required IMutableSetting<DisplayServer.WindowMode> WindowMode { get; init; }

    [Inject(GameSettings.Msaa3D)]
    public required IMutableSetting<Viewport.Msaa> Msaa3D { get; init; }

    [Inject(ProjectSettings.Physics.Gravity2D)]
    public required ISetting<float> Gravity2D { get; init; }

    private void OnVolumeSliderChanged(float value) => MasterVolume.Set(value);

    private void OnFullscreenToggled(bool enabled) =>
        WindowMode.Set(enabled ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
}
```

For a custom setting these methods don't cover, call `AddSetting` directly with a factory for the
adapter - a `Func<IServiceProvider, IMutableSetting<T>>` also registers the plain `ISetting<T>`
alias, the same as the methods above; a `Func<IServiceProvider, ISetting<T>>` registers only the
read-only interface. For a setting with nothing to back it but memory, pass a plain initial value
instead of a factory:

```csharp
services.AddSetting<int>(SettingKeys.Difficulty, initialValue: 1);
```

## Using an engine adapter directly

`IRaycaster`, `IAssetLoader`, `ISceneNavigator`, and `IClock` are ordinary injected services,
resolvable like any other - unlike the input interfaces above, they aren't bound to a key, since
there's only ever one engine to query:

```csharp
public partial class Player : CharacterBody3D
{
    [Inject]
    public required IRaycaster Raycaster { get; init; }

    [Inject]
    public required IClock Clock { get; init; }

    private ulong lastDashMsec;

    private void CheckGround()
    {
        Vector3? hit = Raycaster.Cast(GetWorld3D(), GlobalPosition, GlobalPosition + Vector3.Down * 10, [GetRid()]);
    }

    private bool CanDash() => Clock.ElapsedMilliseconds - lastDashMsec > 1000;
}
```

## Scanning services across multiple assemblies

A project split across several assemblies - a shared gameplay library referenced by the main game
assembly, say - marks each one that declares its own convention-based services:

```csharp
// In the shared gameplay library's own assembly:
[assembly: ConventionScannable]
```

`ServiceManager` scans every currently loaded assembly marked this way, so a convention-named
service or node singleton in either assembly is registered exactly as it would be if the whole
project were a single assembly - no change to the project's own `ServiceManager` subclass is
needed.

## Managing the container's lifetime directly

A project that would rather not extend `ServiceManager` - because its autoload already extends
some other base class, say - can construct a `ServiceContainer` directly instead, the same way
`ServiceManager` does internally:

```csharp
public partial class GameServices : Node
{
    private ServiceContainer container = null!;

    public override void _EnterTree() => container = new ServiceContainer(GetTree(), Configure);

    public override void _ExitTree() => container.Dispose();

    private void Configure(IServiceCollection services) =>
        services.AddScene(SceneKeys.MainMenu, "res://Scenes/MainMenu.tscn");
}
```
