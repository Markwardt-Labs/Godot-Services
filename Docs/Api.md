# API

The public API's primary entry point is `ServiceManager`, a `Node` that a consuming project
extends for its own autoload. Everything else - `[Inject]`, `[Transient]`, `[ConventionScannable]`,
`ServiceContainer`, the `IAsset<T>`/`ISceneAsset` asset interfaces, the
`IInput`/`IInputVector`/`IInputAxis` input interfaces and their `IRebindableInput`/
`IRebindableInputVector`/`IRebindableInputAxis` counterparts, the `ISetting<T>`/
`IMutableSetting<T>` setting interfaces and the `GameSettings`/`ProjectSettings` key classes for
this library's own settings, the `IAssetLoader`/`ISceneNavigator`/`IRaycaster`/`IClock` engine
adapters, and the registration extensions on `ServiceCollectionExtensions` - exists to be composed
by or resolved through it.

## Shape

A consuming project marks its own assembly `[assembly: ConventionScannable]` (typically in an
`AssemblyInfo.cs`), then extends `ServiceManager` as the script on an autoload node, overriding
`Configure` for anything convention scanning can't cover - keyed asset registrations chief among
them:

```csharp
[assembly: ConventionScannable]

public partial class GameServices : ServiceManager
{
    protected override void Configure(IServiceCollection services) =>
        services.AddScene(SceneKeys.MainMenu, "res://Scenes/MainMenu.tscn");
}
```

Scanning every loaded, marked assembly - rather than requiring the caller to enumerate them - lets
a project split its services across several assemblies (a shared library referenced by more than
one game, say) without any extra wiring: each referenced assembly just marks itself, and
`ServiceManager` finds it.

`ServiceManager` itself is a thin `Node` shell: its `_EnterTree`/`_ExitTree` construct and dispose
a `ServiceContainer`, the type that actually touches a live `SceneTree` and does the composition
work. Everything `ServiceContainer` composes internally (`NodeInjector`, `NodeSingletonRegistry`,
the convention scanners) stays plain, engine-decoupled C#. `ServiceContainer` is available
directly too, for a project that would rather manage its own lifetime than extend `ServiceManager`
- construct one from any node's `_EnterTree` and call `Dispose()` from `_ExitTree`, the same shape
`ServiceManager` itself follows.

## Flow

1. `ServiceManager._EnterTree` constructs a `ServiceContainer`, passing it `Configure` as the
   callback for anything convention scanning can't cover.
2. The `ServiceContainer` constructor finds every currently loaded assembly marked
   `[ConventionScannable]` - including this library's own - and, for each in turn, registers that
   assembly's convention-matched services and node-backed singletons (`AddConventionServices`),
   then invokes `Configure`.
3. It builds the `IServiceProvider`, exposed as `ServiceContainer.Provider` (and, in turn,
   `ServiceManager.Provider`).
4. It subscribes to the scene tree's `NodeAdded` signal. From then on, every node that enters the
   tree - during initial startup and for the rest of the session - is captured (if it backs a
   node-registered singleton) and has its `[Inject]`-marked properties populated, in that order,
   before that node's own `_Ready` runs.
5. `ServiceManager._ExitTree` disposes the `ServiceContainer`, which unsubscribes from `NodeAdded`.

## Registering services: convention over configuration

Any concrete, non-generic, non-`Node` class that implements an interface named after itself (e.g.
`WorldService` implementing `IWorldService`) is registered as a singleton automatically - no
attributes or explicit calls needed. Annotate a class with `[Transient]` to register it as
transient instead. The same convention applies to `Node` subclasses, registered instead as
node-backed singletons (see "Node-backed services" below).

## Resolving into nodes: `[Inject]`

Godot always constructs a `Node`-derived script through its parameterless constructor when a
scene loads, so the container can never run a node's own constructor. Nodes instead declare
public settable properties marked `[Inject]`, as `required` with an `init` setter rather than a
plain settable property defaulted to `null!`:

```csharp
[Inject]
public required IWorldService WorldService { get; init; }
```

Only public properties are supported - this keeps a node fully constructible and settable from a
plain unit test without needing the DI container (via an object initializer, since `required`
enforces that at every ordinary construction site - reflection, which is how `[Inject]` itself
populates the property, isn't subject to that same enforcement). `[Inject]` also accepts an
optional key (`[Inject("SomeKey")]`), resolved against a keyed registration instead of the default
one - this is how each individually keyed asset (see "Assets" below) gets resolved.

## Node-backed services

Some services must be backed by a specific autoloaded node rather than a plain class, because
they need to live in the scene tree to render UI or otherwise interact with the engine.
Following the same naming convention as plain services (a `Node` subclass implementing an
interface named after itself) registers it as a node-backed singleton: the interface resolves to
whichever instance of that node type actually entered the scene tree, once it has. Resolving one
before its backing node has entered the tree throws.

## Assets: `IAsset<T>`/`ISceneAsset`

`IAsset<T>` is a single `Task<T> Load()` method - an asset whose path is already known, so call
sites don't thread a path string through their own code. `ISceneAsset` extends `IAsset<PackedScene>`
with `Instantiate<TNode>()` (loads and instantiates the scene's root node) and `Open()` (loads and
changes the active scene to it, via `ISceneNavigator`).

Register one with `AddAsset<TResource>`/`AddScene`, each of which registers a keyed singleton
built from `IAssetLoader` (and, for scenes, `ISceneNavigator`) resolved from the container;
`AddScene` also registers the same instance again under the plain `IAsset<PackedScene>` service
type for the same key:

```csharp
services.AddAsset<Texture2D>(TextureKeys.PlayerTemplate, "res://Assets/Sprites/Player.png");
services.AddScene(SceneKeys.MainMenu, "res://Scenes/MainMenu.tscn");
```

A call site resolves one by key with `[Inject]`'s optional key argument - injecting `ISceneAsset`
for a call site that instantiates or opens the scene, or the plain `IAsset<PackedScene>` for one
that only ever loads it:

```csharp
[Inject(TextureKeys.PlayerTemplate)]
public required IAsset<Texture2D> PlayerTemplate { get; init; }

[Inject(SceneKeys.MainMenu)]
public required IAsset<PackedScene> MainMenu { get; init; }
```

A consuming project typically declares its own `internal static class` of `const string` keys per
asset kind (as in the examples above) - `const` specifically because `[Inject]`'s constructor
argument must be a compile-time constant.

## Input: `IInput`/`IInputVector`/`IInputAxis`

Input is bound the same way an asset is: each interface wraps a query that's already tied to a
specific input action name (or names), so call sites don't thread action name strings through
their own code. `IInput` queries a single action (`IsPressed()`, `IsJustPressed()`,
`IsJustReleased()`, `Strength`); `IInputVector` combines two opposing action pairs into a
deadzone-applied `Vector2`; `IInputAxis` combines one opposing action pair into a `float`.

Register one with `AddInput`/`AddInputVector`/`AddInputAxis`, each of which registers a keyed
singleton bound to the given action name(s):

```csharp
services.AddInput(ActionKeys.Jump, "jump");
services.AddInputVector(ActionKeys.Move, "move_left", "move_right", "move_up", "move_down");
services.AddInputAxis(ActionKeys.Zoom, "zoom_out", "zoom_in");
```

Each registers its rebindable counterpart - `IRebindableInput`, `IRebindableInputVector`,
`IRebindableInputAxis` - extending the plain interface with one `Rebind*` method per underlying
action (`Rebind(InputEvent)` for a single action; `RebindNegativeX`/`RebindPositiveX`/
`RebindNegativeY`/`RebindPositiveY` for a vector; `RebindNegative`/`RebindPositive` for an axis),
since each action can be rebound independently. Each also registers the same instance again under
its plain interface for the same key, so a call site resolves either one by key with `[Inject]`'s
optional key argument, the same as an asset - injecting the rebindable interface for a settings
screen that lets the player remap the action, or the plain interface for a call site that only
ever reads it:

```csharp
[Inject(ActionKeys.Jump)]
public required IRebindableInput Jump { get; init; }

private void OnJumpKeyCaptured(InputEvent newBinding) => Jump.Rebind(newBinding);
```

```csharp
[Inject(ActionKeys.Jump)]
public required IInput Jump { get; init; }
```

## Settings: `ISetting<T>`/`IMutableSetting<T>`

`ISetting<T>` is a bound, reusable `Get()` - a setting whose name is already known, the same
"bound by key" shape as an asset or an input action. `IMutableSetting<T> : ISetting<T>` extends it
with `Set(T value)`, the same plain/rebindable split as `IInput`/`IRebindableInput`. Neither
interface constrains `T` itself - only the specific implementations that actually marshal it
through the engine as a `Variant` (the ones backing `AddProjectSetting`/`AddShaderSetting` below)
require `T` to satisfy Godot's `[MustBeVariant]` constraint, the same requirement
`Godot.Collections.Array<T>`/`Dictionary<TKey, TValue>` carry.

`IMutableSetting<T>` backs several different kinds of engine setting - every registration method
registers its concrete adapter under this mutable interface, since there's no read-only
implementation to register instead, and also registers the same instance again under the plain
`ISetting<T>` for the same key. For a project setting, a global shader parameter, or an audio
bus's volume/mute state, the consuming project supplies its own key and the setting's name - the
same shape as registering an asset:

```csharp
services.AddProjectSetting<float>(SettingKeys.MasterVolume, "audio/master_volume");
services.AddShaderSetting<Color>(SettingKeys.FogColor, "fog_color");
services.AddVolumeSetting(SettingKeys.MusicVolume, "Music");
services.AddMuteSetting(SettingKeys.MusicMuted, "Music");
```

For a fixed, singular setting - the main window's display mode, vertical sync mode, size, current
screen, and UI scale factor; MSAA, screen-space AA, and 3D resolution scaling; the audio
output/input device; the mouse mode; the engine's max-FPS cap; the active locale - there's exactly
one of each, so `ServiceContainer` registers all of them automatically, under this library's own
well-known keys in `GameSettings` (e.g. `GameSettings.WindowMode`, `GameSettings.Msaa3D`), the
same way it registers `IClock`/`IAssetLoader`/etc.; no registration call is needed for them. It
does the same for every path in `ProjectSettings`, the library's curated list of
commonly-configured project settings - each path is already a stable, unique key, so it registers
itself under its own path with no project-supplied key needed either.

A call site resolves either service type by key with `[Inject]`, the same as an asset or an input
action - injecting `IMutableSetting<T>` for a settings screen that writes the value, or the plain
`ISetting<T>` for a call site that only ever reads it. It neither knows nor cares which kind of
setting is actually behind the interface:

```csharp
[Inject(SettingKeys.MasterVolume)]
public required IMutableSetting<float> MasterVolume { get; init; }

[Inject(GameSettings.WindowMode)]
public required IMutableSetting<DisplayServer.WindowMode> WindowMode { get; init; }

[Inject(ProjectSettings.Physics.Gravity2D)]
public required ISetting<float> Gravity2D { get; init; }

private void OnVolumeSliderChanged(float value) => MasterVolume.Set(value);
```

A project with its own setting adapter - one none of the above methods cover - registers it
directly with `AddSetting`: pass a `Func<IServiceProvider, ISetting<T>>` to register a read-only
adapter with no alias, or a `Func<IServiceProvider, IMutableSetting<T>>` to register a mutable one
that also aliases the plain `ISetting<T>`, the same way the built-in methods do. For a setting with
nothing to back it but memory, pass a plain initial value instead of a factory:

```csharp
services.AddSetting<int>(SettingKeys.Difficulty, initialValue: 1);
services.AddSetting<int>(SettingKeys.HighScore, provider => new HighScoreSetting(provider.GetRequiredService<ISaveFile>()));
```

## Engine adapters

`IAssetLoader`, `ISceneNavigator`, `IRaycaster`, and `IClock` each wrap one otherwise-unmockable
concrete engine API - threaded resource loading, the global scene tree, 3D physics ray casts, and
the engine/system clock, respectively - behind an injectable interface. Unlike the keyed input
interfaces above, each of these resolves to a single, unkeyed instance - there's exactly one
engine to load resources from, navigate, or read the time from, so no binding key is needed. All
four are registered automatically, since this library's own assembly is itself marked
`[ConventionScannable]`; a consuming project depends on the interface like any other service, and
never needs to touch the concrete adapter directly.
