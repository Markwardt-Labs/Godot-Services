Covers `ISetting<T>`/`IMutableSetting<T>`, `ProjectSetting<T>`, `ShaderGlobal<T>`, `AudioVolume`,
`AudioMute`, `AudioOutputDevice`, `AudioInputDevice`, `WindowMode`, `VSyncMode`, `WindowSize`,
`CurrentScreen`, `Msaa2D`, `Msaa3D`, `ScreenSpaceAA`, `Scaling3DMode`, `Scaling3DScale`,
`ContentScaleFactor`, `MouseMode`, `MaxFps`, `Locale`, `InMemorySetting<T>`, `GameSettings`,
`ProjectSettings`, and `ServiceCollectionExtensions`'s `AddSetting`/`AddProjectSetting`/
`AddShaderSetting`/`AddVolumeSetting`/`AddMuteSetting` methods: the mechanics behind a keyed,
bound setting registration.

## One concrete adapter, registered under both its mutable and plain interface

`IMutableSetting<T> : ISetting<T>` adds `Set(T value)` to `ISetting<T>`'s `Get()`, the same split
as `IRebindableInput : IInput`. Every built-in setting adapter implements `IMutableSetting<T>`
directly (e.g. `AudioVolume : IMutableSetting<float>`), and `AddProjectSetting`/`AddShaderSetting`/
`AddVolumeSetting`/`AddMuteSetting` all funnel through the `Func<IMutableSetting<T>>` overload of
`AddSetting` - there's no separate, read-only implementation for any of them. That overload
registers the concrete type under the mutable service type, then registers the *same instance*
again under the plain `ISetting<T>` service type for the same key, via a private
`AddKeyedAlias<TBase, TDerived>` helper whose factory resolves the already-registered mutable
singleton rather than constructing a second instance. This exists because
`Microsoft.Extensions.DependencyInjection`'s keyed resolution matches a service's exact declared
type, not any assignable base or interface - a property declared `ISetting<T>` for a setting
registered only under `IMutableSetting<T>` would fail to resolve. A call site that only ever reads
a value can now inject the narrower `ISetting<T>` directly, rather than injecting the mutable
interface and narrowing to it in its own code.

## `AddSetting`'s overloads mirror the mutable/plain split

A consuming project with its own setting adapter - one this library doesn't already cover - calls
`AddSetting` directly rather than reaching for the lower-level `AddKeyedSingleton` and reimplementing
the aliasing dance by hand. Its `Func<ISetting<T>>` overload registers exactly what it's given, with
no alias, since a factory that only promises `ISetting<T>` might genuinely have nothing to alias -
there's no `Set` to expose. Its `Func<IMutableSetting<T>>` overload behaves exactly like the
built-in registration methods, aliasing the same instance under `ISetting<T>` too. Overload
resolution picks between the two the same way it would for any other overloaded method: a factory
lambda whose inferred return type implements `IMutableSetting<T>` binds to that overload (the more
specific target), so a custom mutable adapter gets the alias without the caller having to pick the
overload explicitly.

A third overload takes a plain `T initialValue` instead of a factory, for a setting with nothing
to back it but memory - no project setting, shader global, or engine API involved, just a value a
project wants to store and share by key (a difficulty level, say). It registers an
`InMemorySetting<T>`, which starts at `initialValue` and otherwise behaves like any other
`IMutableSetting<T>`, aliased under `ISetting<T>` the same way. Unlike every other adapter in this
library, `InMemorySetting<T>` touches no engine API at all, so it's exercised directly by real unit
tests rather than being `[ExcludeFromCodeCoverage]`.

## Many implementations behind that one interface

Regardless of which engine API actually backs it or how that API is shaped, every setting adapter
converges on the same `Get()`/`Set(T value)` shape. `ProjectSettings.GetSetting`/`SetSetting` and
`RenderingServer.GlobalShaderParameterGet`/`Set` both already take a plain name and a `Variant`,
so `ProjectSetting<T>`/`ShaderGlobal<T>` wrap them almost directly. `AudioVolume`/`AudioMute`
additionally resolve `AudioServer.GetBusIndex(bus)` on every `Get()`/`Set()` call rather than
caching the index at construction, since a bus's index can change if the project's bus layout is
edited at runtime (in an audio settings screen, for instance) - caching it would silently start
reading or writing the wrong bus after such a change.
`WindowMode`/`VSyncMode`/`WindowSize`/`CurrentScreen`/`AudioOutputDevice`/`AudioInputDevice`/
`MouseMode`/`MaxFps`/`Locale` each wrap a single `DisplayServer`/`AudioServer`/`Input`/`Engine`/
`TranslationServer` call that takes no name at all. `Msaa2D`/`Msaa3D`/`ScreenSpaceAA`/
`Scaling3DMode`/`Scaling3DScale`/`ContentScaleFactor` are the exception to "wraps a static API":
each is a plain property on the root `Window`/`Viewport` rather than a static entry point, so each
takes it as a constructor parameter instead of needing no state at all - see below. None of that
variation is visible to a caller holding an `ISetting<T>`/`IMutableSetting<T>` - it only ever sees
`Get()`/`Set(value)`.

## Viewport/window-backed settings need a live instance, not just a static call

`Viewport.Msaa2D`/`Msaa3D`/`ScreenSpaceAA`/`Scaling3DMode`/`Scaling3DScale` and
`Window.ContentScaleFactor` are instance properties on the current root window, unlike every other
engine API this library's adapters wrap - `SceneTree.Root` is itself typed `Window`, which is a
`Viewport`, so one captured reference serves both kinds of adapter. `ServiceContainer` captures
`tree.Root` once into a local variable inside `AddSettings` and passes it to each of these six
constructors - the same root instance is safe to reuse for all of them, and for the life of the
`ServiceContainer`, because `SceneNavigator.ChangeScene` (`SceneTree.ChangeSceneToPacked`) only
ever swaps `SceneTree.CurrentScene`, a child node under the root; the root `Window` itself never
changes. `ServiceContainer` doesn't register `Window`/`Viewport` itself as a container-wide
singleton - a consuming project might reasonably want its own keyed `Viewport` registrations for
sub-viewports, and an ambient, unkeyed registration would collide with that. Capturing it locally
and closing over it in each factory keeps the root window a private implementation detail of these
six registrations, not a new part of the public service graph.

## Why `[MustBeVariant]` is on specific implementations, not the interfaces

`ISetting<T>`/`IMutableSetting<T>` declare a plain, unconstrained `T` - marshalling through the
engine as a `Variant` is how `ProjectSetting<T>`/`ShaderGlobal<T>` happen to implement `Get()`/
`Set(T value)`, not a property of "being a setting" in general. Only the implementations (and
registration methods) that actually call `Variant.From<T>()`/`.As<T>()` declare
`[MustBeVariant] T` themselves - `ProjectSetting<T>`, `ShaderGlobal<T>`, and
`AddProjectSetting`/`AddShaderSetting` - so the constraint sits exactly where the real requirement
is. Putting it on `ISetting<T>` instead would force every custom setting adapter's `T` to be
Variant-compatible even for one that never touches a `Variant` at all (wrapping a plain in-memory
value, say), which is exactly the kind of adapter `AddSetting`'s general, unconstrained overloads
exist to support. `AudioVolume`, `AudioMute`, `AudioOutputDevice`, `AudioInputDevice`, `WindowMode`,
`VSyncMode`, `WindowSize`, `CurrentScreen`, `Msaa2D`, `Msaa3D`, `ScreenSpaceAA`, `Scaling3DMode`,
`Scaling3DScale`, `ContentScaleFactor`, `MouseMode`, `MaxFps`, and `Locale` don't declare it
either, for the same reason they never did: each closes `IMutableSetting<T>` over a single
concrete type directly, with no open type parameter of its own for the constraint to attach to.

## Library-provided settings register themselves, project-provided ones need a call per setting

`AddProjectSetting`/`AddShaderSetting`/`AddVolumeSetting`/`AddMuteSetting` each register exactly
one setting per call, because each needs a name or bus the calling project supplies - there's no
way to guess in advance which project settings, shader globals, or audio buses a given project
actually has. The library's own fixed, singular settings, by contrast, need no call at all:
`ServiceContainer` registers every `GameSettings` key itself, in its own constructor, because
there's nothing left for a caller to supply. It does the same for every key in `ProjectSettings` -
each one is already a stable, unique engine path, so it doubles as its own registration key and
needs no project-supplied name either; a consuming project only calls `AddProjectSetting` itself
for a project setting outside that list.
