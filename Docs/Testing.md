# Testing

Unit tests run via plain `dotnet test` (xUnit), not inside a running Godot engine process. This
has one hard consequence that shapes every test in this repository: **a real `new SomeNode()` (or
the construction of any other natively-backed Godot type) segfaults the whole test process outside
a running engine.** This isn't limited to `GodotObject` subclasses like `Node`/`Resource` - a
plain-`object`-derived wrapper like `StringName` (used to bind an input action by name) is backed
by the same kind of native interop and crashes identically. Every such constructor calls into
native code to allocate or intern its engine-side counterpart, and there is no engine hosting the
process under `dotnet test`.

## Constructing a fixture without the engine

`Tests/src/GodotObjects.cs` provides `GodotObjects.CreateUninitialized<T>()`, which uses
`RuntimeHelpers.GetUninitializedObject` to allocate a natively-backed instance without running its
constructor chain - no native call ever happens. The result is safe for plain reflection (read its
`Type`, get/set a plain C# property) or to pass as an otherwise-unused argument, but must never
have an actual engine API invoked on it (`Node.AddChild`, `PackedScene.Instantiate`, etc.) - those
still reach into the native object this trick never allocated, and will segfault just the same.
Always use this helper instead of `new()` for a `Node`/`Resource`/`PackedScene`/`StringName`
fixture in a unit test. This is also why every registration extension method
(`AddInput`, `AddShaderSetting`, `AddVolumeSetting`, etc.) takes a plain `string` rather than a
`StringName` for the name it binds to - a test exercising one of these never needs to construct a
`StringName` at all, uninitialized or otherwise.

## What can and can't be covered

- Pure reflection over `Type` (convention scanning in `ServiceCollectionExtensions`/
  `NodeSingletonRegistry`, `NodeInjector.GetInjectableProperties`) never constructs anything and
  is always safe.
- Registering a service (`AddSingleton`, `AddKeyedSingleton`, etc.) doesn't invoke its factory
  until something resolves it, so registration-only assertions are always safe too.
- `NodeInjector.Inject`/`NodeSingletonRegistry.Capture` need a real object to operate on, but only
  ever call plain reflection (`PropertyInfo.SetValue`) or dictionary lookups on it - safe with a
  `GodotObjects.CreateUninitialized<T>()` fixture.
- `ISceneAsset.Instantiate` calls the real `PackedScene.Instantiate`, which needs actual native
  scene data to do anything meaningful; there's no safe way to fake that outside the engine, so
  it isn't covered by a unit test.
- `Adapters` (`AssetLoader`, `SceneNavigator`, `Raycaster`, `Clock`, `Input`, `InputVector`,
  `InputAxis`, `ProjectSetting<T>`, `ShaderGlobal<T>`, `AudioVolume`, `AudioMute`, `WindowMode`,
  `VSyncMode`, `WindowSize`, `CurrentScreen`, `Msaa2D`, `Msaa3D`, `ScreenSpaceAA`,
  `Scaling3DMode`, `Scaling3DScale`, `ContentScaleFactor`, `AudioOutputDevice`,
  `AudioInputDevice`, `MouseMode`, `MaxFps`, `Locale`) each exist purely to wrap one
  otherwise-unmockable concrete engine API behind an interface, so invoking them for real in a
  test would mean doing the exact unsafe thing they exist to let other code avoid. Each is
  marked `[ExcludeFromCodeCoverage]`;
  `ServiceContainer` and `ServiceManager` likewise, since both touch a live `SceneTree`
  (`ServiceContainer` subscribes to its `NodeAdded` signal directly; `ServiceManager` builds one
  via `Setup` from `_EnterTree`/`_ExitTree`, and calls `CallDeferred` besides, but is itself a
  `Node` that can't be constructed outside the engine either) - all matched here and in the
  Cobertura report.
- `InMemorySetting<T>` is the one setting adapter that isn't `[ExcludeFromCodeCoverage]`: it stores
  its value purely in memory rather than reading/writing any engine API, so it's exercised by real
  unit tests like any other plain C# class.

Coverage is collected via `dotnet test --collect:"XPlat Code Coverage"` against
`Tests/coverage.runsettings`. Godot's source generator also emits a `GodotPlugins.Game.Main` entry
point into the compiled assembly; `-classfilters:-GodotPlugins.Game.Main` on every
`reportgenerator` call excludes it, since it isn't code of ours to cover.
