# Architecture

This document explains the high-level design decisions behind the library's implementation - why
it's built the way it is, not the class-by-class mechanics of how.

## Convention-based registration over explicit wiring

A service is registered by naming convention (`Thing` implementing `IThing`) rather than through
an explicit registration call or attribute per service. The alternative - a `[Service]` attribute,
or a hand-maintained list of `services.AddSingleton<IThing, Thing>()` calls - adds a line of
ceremony to every single service a project defines, for a rule that's almost always true anyway
(a service and its contract share a name). The convention costs an assembly scan at startup and
a slightly less greppable registration site; both are cheap compared to the ceremony saved across
a project's entire service surface. `[Transient]` exists as an explicit opt-out precisely because
it's the *exception* - singleton is the overwhelmingly common case for a service's lifetime in a
single-process game, so it's the unmarked default.

## Assembly discovery via attribute, not an explicit list

`ServiceContainer` finds which assemblies to scan by looking for `[ConventionScannable]` across
every currently loaded assembly (`AppDomain.CurrentDomain.GetAssemblies()`), rather than taking an
explicit list of assemblies as a constructor argument. An explicit list was the first design:
simpler to trace from the call site, but it pushed the same bookkeeping problem the convention
scanner already exists to avoid up one level - a consuming project would need to remember to add
every new referenced assembly (this library's own included) to that list by hand, and a forgotten
one would fail silently (no error, just a missing registration). Marking an assembly with the
attribute is a one-time, self-contained declaration that travels with the assembly itself - a
project that references a `[ConventionScannable]` library gets its services for free, with nothing
to keep in sync at the `ServiceContainer` call site.

## Property injection for nodes, constructor injection for everything else

Godot always constructs a `Node`-derived script through a parameterless constructor when a scene
loads (godotengine/godot#15434); there is no supported way to run a container-provided constructor
for it. Two options remained: a service-locator call inside each node (`Services.Get<IThing>()`),
or public settable properties populated after construction. The service-locator approach was
rejected because it hides a node's actual dependencies behind a string/type lookup buried in its
body, rather than declaring them at the type's boundary where a reader (or a test) can see them at
a glance. Property injection keeps dependencies declared as ordinary properties, fully assignable
by hand from a unit test without touching the DI container. Declaring them `required` with an
`init` setter, rather than a plain settable property defaulted to `null!`, costs nothing here:
`required`/`init` are enforced by the C# compiler only at an ordinary `new` call site, never by the
runtime, so `NodeInjector`'s reflection-based `PropertyInfo.SetValue` - which runs after Godot's
own parameterless construction, entirely outside any C# `new` expression - populates an `init`
property exactly as freely as it would a plain settable one, while a test can still assign one
directly through an object initializer. Only public properties are supported specifically to
preserve that test-time assignability - a private or internal `[Inject]` property would defeat the
point by resurrecting the same container dependency it exists to avoid.

## `ServiceContainer` as the one engine-coupled composition root

Every other type in this library - the convention scanners, `NodeInjector`, `NodeSingletonRegistry`
- is plain C# with no dependency on a *live* engine object, only on Godot's static types
(`Node`, `Resource`, etc.) for their signatures. `ServiceContainer` is the sole exception: it
holds the actual `SceneTree` and subscribes to its `NodeAdded` signal. Concentrating that one
stateful, engine-coupled responsibility in a single type - rather than spreading scene-tree
subscription logic across several - means a consuming project has exactly one lifetime to manage
(construct in `_EnterTree`, dispose in `_ExitTree`), and everything that type composes internally
stays independently exercisable without a running engine.

## `ServiceManager` wraps `ServiceContainer` instead of replacing it

`ServiceContainer` deliberately isn't a `Node` itself - it only *needs* a `SceneTree` reference and
a place to call `Dispose()`, not scene-tree membership of its own. `ServiceManager` is a thin
`Node` subclass built on top of it, handling `_EnterTree`/`_ExitTree` and exposing `Configure` as
a virtual hook, purely for the common case: a project whose autoload has no other base class to
extend just extends `ServiceManager` and overrides `Configure`, with no `ServiceContainer` field
or lifetime management of its own to write. Folding `ServiceContainer`'s logic directly into
`ServiceManager` - making the `Node` subclass the *only* entry point - was rejected because it
would strand a project whose autoload already needs to extend something else (or that wants finer
control over exactly when the container is constructed and disposed, decoupled from
`_EnterTree`/`_ExitTree`'s timing) with no way to use this library at all. Keeping the two
separate means the ergonomic path (`ServiceManager`) costs a project nothing extra, while the
unopinionated path (`ServiceContainer` constructed and owned by hand) stays fully available.

## Adapters wrap engine statics instead of call sites depending on them directly

`ResourceLoader`, `Engine.GetMainLoop()`, `PhysicsDirectSpaceState3D`, `Input`, `InputMap`, `Time`,
`ProjectSettings`, `AudioServer`, `DisplayServer`, `Engine`, `TranslationServer`, and
`RenderingServer`'s global shader parameter API are each a static or otherwise-global entry point
into engine state with no interface of their own. A call site that depends on one directly can
never substitute a fake for it. Each adapter (`AssetLoader`, `SceneNavigator`, `Raycaster`,
`Input`, `InputVector`, `InputAxis`, `Clock`, `ProjectSetting<T>`, `ShaderGlobal<T>`,
`AudioVolume`, `AudioMute`, `WindowMode`, `VSyncMode`, `WindowSize`, `CurrentScreen`,
`AudioOutputDevice`, `AudioInputDevice`, `MouseMode`, `MaxFps`, `Locale`) exists purely to give
that one concrete call an interface boundary, so everything built on top of it - `ResourceAsset<T>`,
`SceneAsset` - depends on the interface instead and stays substitutable. `Msaa2D`, `Msaa3D`,
`ScreenSpaceAA`, `Scaling3DMode`, `Scaling3DScale`, and `ContentScaleFactor` are the exception:
they wrap instance properties on the root `Window` (also a `Viewport`) rather than a static, so
each takes it as a constructor parameter, supplied by `ServiceContainer` (the only place that
already holds a live `SceneTree` to get it from) instead of needing no state at all.

## `string`, not `StringName`, in registration method signatures

`AddInput`/`AddInputVector`/`AddInputAxis`/`AddShaderSetting`/`AddVolumeSetting`/`AddMuteSetting` all
take a plain `string` for the action/parameter/bus name they bind to, even though the engine APIs
underneath (`Input`, `InputMap`, `RenderingServer`, `AudioServer`) take `StringName`. `StringName`
is itself natively backed - constructing one calls into the engine the exact same way constructing
a `Node` does, so exposing it in a public signature would mean a consuming project's own test code
could hit the same construct-outside-the-engine crash this library's own tests have to work around
with `GodotObjects.CreateUninitialized<T>()`. A plain `string` needs no such workaround for a
caller, and still converts to `StringName` implicitly at the one call site inside each adapter
that actually needs it - converted once, at construction, and reused for that adapter's lifetime
rather than reconverted on every `Get()`/`IsPressed()`/etc. call.

## Input is keyed like an asset, `Clock` isn't

`IInput`/`IInputVector`/`IInputAxis` are registered as *keyed* singletons, bound to a
specific action name at registration time, the same as `IAsset<T>` is bound to a specific path -
there can be any number of distinct input bindings a project cares about ("jump", "move",
"interact", ...), each independently resolved by its own key. `IClock`, by contrast, is a single
unkeyed singleton: there is exactly one engine clock, with no equivalent "which one" question to
answer, so keying it would only add ceremony (a key every call site would have to spell out and
get right) without giving a caller any actual choice. The same reasoning already separates
`IAssetLoader`/`ISceneNavigator` (unkeyed - one engine to load from or navigate) from `IAsset<T>`/
`ISceneAsset` (keyed - many distinct assets); input follows whichever side of that line each
interface's own cardinality puts it on.

## This library's own singular settings are keyed too, with library-provided keys

The main window's display mode/vsync mode and the engine's max-FPS cap are each singular, the same
as `IClock` - there's only ever one of each. The first design followed `IClock`'s precedent
exactly: a dedicated marker interface per setting (`IWindowMode : IMutableSetting<DisplayServer.WindowMode>`,
with no members of its own beyond what `IMutableSetting<T>` already declares) so each could be
convention-registered and resolved unkeyed. That was rejected as three needless interfaces adding
no real contract of their own - `IMutableSetting<DisplayServer.WindowMode>` already says
everything `IWindowMode` would, and the *only* reason to introduce it was to give the convention
scanner a name to match, not because callers needed a distinct type. Keeping every mutable setting
on the single `IMutableSetting<T>` interface, and using a *key* to distinguish "which setting" -
the same mechanism already used for project settings, shader globals, and per-project input
actions - avoids that, but reintroduces the exact problem convention registration exists to
solve: something still has to actually call `AddKeyedSingleton` for each one. Since these settings
are this library's own rather than a consuming project's, `ServiceContainer` registers every one
of them directly in its own constructor, the same as it does for `IClock`/`IAssetLoader`/etc. - a
consuming project never calls anything to get them, it only ever resolves them, by this library's
own well-known keys (`GameSettings.WindowMode`, `GameSettings.VSyncMode`, `GameSettings.MaxFps`,
and so on), shipped as public constants rather than something every consuming project would
otherwise have to invent and keep in sync itself.

## `IRebindableInput` extends `IInput`, and `AddInput` registers both

`IRebindableInput`/`IRebindableInputVector`/`IRebindableInputAxis` each extend their read-only
counterpart rather than being separate, unrelated interfaces - a rebindable input already *is* a
readable one. The first design paired each with its own registration method too
(`AddInput`/`AddRebindableInput`, and so on for the vector/axis shapes), so a consumer could opt
into rebinding only where needed. That was rejected: `AddInput`/`AddRebindableInput` construct the
exact same concrete adapter (`Input`) either way - there's no smaller object being built for the
"just reading" case, only a narrower interface it's exposed through - so the second method bought
interface segregation at the cost of doubling the registration surface (six methods instead of
three) for a distinction with no underlying cost difference. `AddInput`/`AddInputVector`/
`AddInputAxis`'s action/name-taking overload now registers the rebindable interface, and also
aliases the same instance under the plain `IInput`/`IInputVector`/`IInputAxis` service type for
the same key, keeping one method per input shape while still letting DI resolution target either
interface directly - a consumer no longer has to inject the rebindable interface and narrow it in
its own code just to depend on the read contract.
