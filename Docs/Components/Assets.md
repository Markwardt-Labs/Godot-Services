Covers `ResourceAsset<TResource>`, `SceneAsset`, and `ServiceCollectionExtensions`'s `AddAsset`/
`AddScene` methods: the mechanics behind a keyed, lazily-loadable asset registration.

## Why `ResourceAsset<TResource>` isn't sealed

`ResourceAsset<TResource>` is `internal`, not `public`, but deliberately not `sealed` either:
`SceneAsset` extends it with `PackedScene`-specific behavior (`Instantiate`, `Open`) rather than
wrapping a separate `ResourceAsset<PackedScene>` instance and duplicating its `Load()`
implementation. Nothing outside this assembly can subclass it either way, since it's `internal` -
the non-`sealed` choice is purely about code reuse between `ResourceAsset<TResource>` and
`SceneAsset` within this library, not about extensibility for a consuming project.

## A general, factory-taking overload backs the path-taking one

`AddAsset<T>`/`AddScene` each have two overloads: a general one taking a
`Func<IServiceProvider, IAsset<T>>`/`Func<IServiceProvider, ISceneAsset>` factory, and a more
specific one taking a `res://` path, which just calls the general overload with a factory that
builds a `ResourceAsset<TResource>`/`SceneAsset`. Every factory-taking registration method across
this library takes the composed `IServiceProvider`, even the ones (like the setting and input
registrations) whose adapters don't happen to need it, so a caller never has to remember which
family's factory can reach the container and which can't. For `AddAsset<T>`/`AddScene` it's load-
bearing rather than incidental: `ResourceAsset<TResource>`/`SceneAsset` need `IAssetLoader` (and,
for a scene, `ISceneNavigator`) resolved from the container, which a parameterless factory
couldn't reach at all. A project with a custom `IAsset<T>`/`ISceneAsset` implementation of its own
calls the general overload directly, the same way the path-taking one does internally, without
reaching for the lower-level `AddKeyedSingleton` and reimplementing the aliasing dance by hand
(see below).

Either overload registers a factory, not an already-constructed instance - the concrete
`ResourceAsset<TResource>`/`SceneAsset`/custom adapter is constructed the first time something
resolves that key. This keeps asset registration itself cheap and side-effect-free (no resource
actually loads, no engine API is touched, until a caller resolves and calls `Load()`), and means
the factory always sees whatever's actually registered for its dependencies at resolution time,
rather than whatever was registered at the moment `AddAsset`/`AddScene` happened to run.

## `AddScene` also aliases the plain `IAsset<PackedScene>` service type

`ISceneAsset : IAsset<PackedScene>`, so `AddScene` registers the same `SceneAsset` instance a
second time, under the plain `IAsset<PackedScene>` service type for the same key, via a private
`AddKeyedAlias<TBase, TDerived>` helper whose factory resolves the already-registered `ISceneAsset`
singleton rather than constructing a second instance. Without this, a property declared
`IAsset<PackedScene>` for a scene registered only under `ISceneAsset` would fail to resolve -
`Microsoft.Extensions.DependencyInjection`'s keyed resolution matches a service's exact declared
type, not any assignable base or interface. A call site that only needs to load the scene (never
instantiate or open it) can now inject the narrower `IAsset<PackedScene>` directly, the same way an
`AddAsset` registration is injected.

## `Load()` is idempotent per singleton, not per call

Because each `IAsset<T>`/`ISceneAsset` is registered as a keyed *singleton*, the same
`ResourceAsset<TResource>`/`SceneAsset` instance backs every resolution of that key - but `Load()`
itself doesn't cache the loaded resource. Each call re-invokes `IAssetLoader.Load`, which in turn
re-issues a threaded load request to the engine. `ResourceLoader`'s own resource cache (keyed by
`res://` path) is what actually avoids redundant loads across repeated `Load()` calls - this
library doesn't duplicate that caching itself.
