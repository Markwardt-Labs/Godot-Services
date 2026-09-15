Covers `ServiceContainer`, `NodeInjector`, `NodeSingletonRegistry`, `ServiceCollectionExtensions`,
and `ConventionScannableAssemblies`: the mechanics behind convention-based registration, `[Inject]`
property population, and node-backed singleton resolution. `ServiceManager` is a thin `Node`
wrapper around `ServiceContainer` and has no mechanics of its own beyond that delegation.

## Discovering assemblies to scan

`ConventionScannableAssemblies.Discover()` is the one place `AppDomain.CurrentDomain.GetAssemblies()`
is called - both `ServiceContainer`'s constructor and the no-argument
`ServiceCollectionExtensions.AddConventionServices()` overload go through it, rather than each
querying the `AppDomain` themselves. `GetAssemblies()` only returns assemblies the runtime has
already loaded, not everything referenced on disk - in practice this is never a problem for a
Godot game process, since the engine loads the full compiled game assembly (and everything it
statically references, this library included) together at startup, well before a `ServiceContainer`
is ever constructed.

## Capture before inject, every time

`ServiceContainer`'s single `NodeAdded` handler always calls `NodeSingletonRegistry.Capture`
before `NodeInjector.Inject` on the same node. This ordering matters because a node can be both a
node-backed singleton itself *and* have its own `[Inject]` properties - capturing first means a
node that resolves itself (directly or transitively, through another service's constructor) during
its own injection sees itself already captured. `NodeAdded` fires during a node's
`_EnterTree`/scene-tree-attach phase, before `_Ready` runs on anything - so by the time the
*entire* initial tree (every autoload plus the main scene) reaches `_Ready`, every node-backed
singleton in it has already been captured and every `[Inject]` property already populated,
regardless of tree order.

## Reflection is scan-once, resolve-many

`NodeInjector` caches each type's `[Inject]`-marked properties (and validates them) the first time
that type is seen, in a `Dictionary<Type, PropertyInfo[]>` keyed by the node's runtime type - the
validation walk and the `PropertyInfo` array allocation both happen once per distinct node type,
not once per node instance. `NodeSingletonRegistry.RegisterConventionNodes` and
`ServiceCollectionExtensions.AddConventionServices` each do their own single `Assembly.GetTypes()`
pass per assembly; neither caches beyond that, since a given assembly is only ever scanned once
per `ServiceContainer` construction (or per direct call, for a caller using the extension methods
outside `ServiceContainer`).

## Why `Register<TService, TNode>()` is invoked through reflection

`RegisterConventionNodes` discovers `(TService, TNode)` pairs at runtime, as `Type` objects - it
can't call the generic `Register<TService, TNode>()` directly, since C# generic type arguments are
resolved at compile time. It instead looks up `Register`'s `MethodInfo` once (cached as an instance
field, not re-resolved per node type) and calls `MakeGenericMethod(contract, type).Invoke(this,
null)` for each discovered pair - the one place in this library that pays for the assembly-scanning
convention with a fully dynamic dispatch instead of a direct call.

## Resolving before capture throws, on purpose

A node-backed singleton's factory (registered by `Register<TService, TNode>()`) throws
`InvalidOperationException` if its backing node hasn't been captured yet, rather than returning
`null` or blocking. Service resolution is synchronous, with no way to await a future node arrival
mid-resolution, so a premature resolution is a genuine ordering bug in the consuming project
(usually: something in the initial tree resolving a node-backed singleton whose backing autoload
enters the tree later than it). Failing loudly and immediately surfaces that bug at the exact
resolution call site, instead of leaving a caller silently holding `null`.
