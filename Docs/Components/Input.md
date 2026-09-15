Covers `Input`, `InputVector`, `InputAxis`, `IRebindableInput`/`IRebindableInputVector`/
`IRebindableInputAxis`, and `ServiceCollectionExtensions`'s
`AddInput`/`AddInputVector`/`AddInputAxis` methods: the mechanics behind a keyed, bound input
query and rebind.

## Bound at registration, not at the call site

Each type binds its action name(s) once, at construction, rather than taking them as parameters on
`IsPressed()`/`Value`/etc. This mirrors `ResourceAsset<TResource>` binding its path at
construction rather than accepting one on `Load()`: the binding only needs to happen once, at
registration, and every subsequent read is a plain, parameterless query - a call site holding an
`IInput` never has the chance to pass the wrong action name, because there's no name parameter
left to get wrong. Each constructor also converts its `string` parameter(s) to `StringName` once,
storing the result rather than the original `string` - the engine APIs these types wrap
(`Input`/`InputMap`) all take `StringName`, so converting once at construction avoids repeating
that conversion (itself a native call) on every `IsPressed()`/`Value`/`Rebind*` call.

## Three shapes instead of one parameterized type

`IInput`/`IInputVector`/`IInputAxis` are three separate interfaces rather than one generic
`IInput<T>` parameterized over its return shape, because the three wrap fundamentally different
numbers of underlying actions (one, four, and two, respectively) - a single generic interface
would still need three different registration methods and three different constructor shapes
underneath, so collapsing the public interfaces into one wouldn't actually remove any of the real
variation, just hide it behind a shared name.

## Why `IInputVector`'s deadzone defaults to `-1`

`AddInputVector`'s `deadzone` parameter defaults to `-1`, matching `Input.GetVector`'s own default
exactly - a negative deadzone tells the engine to use the average of the four bound actions' own
configured deadzones instead of a caller-supplied one. Most projects configure meaningful
per-action deadzones once, in the InputMap, and want every `GetVector`-style reading to respect
them consistently; requiring every `AddInputVector` call to repeat that value (or guess at one)
would just be one more place for it to drift out of sync with the InputMap.

## One concrete adapter, registered under both its rebindable and plain interface

`Input`, `InputVector`, and `InputAxis` each implement their rebindable interface directly (e.g.
`Input : IRebindableInput`), and their action/name-taking `AddInput`/`AddInputVector`/
`AddInputAxis` overload registers that same concrete type under the rebindable service type -
there's no separate, reading-only implementation for any of the three. That overload also
registers the *same instance* a second time, under the plain `IInput`/`IInputVector`/
`IInputAxis` service type for the same key, via a private `AddKeyedAlias<TBase, TDerived>` helper
whose factory resolves the already-registered rebindable singleton rather than constructing a
second instance. This exists because `Microsoft.Extensions.DependencyInjection`'s keyed
resolution matches a service's exact declared type, not any assignable base or interface - a
property declared `IInput` for an action registered only under `IRebindableInput` would fail to
resolve. A call site that only ever reads a value can now inject the narrower
`IInput`/`IInputVector`/`IInputAxis` directly, rather than injecting the rebindable interface and
narrowing to it in its own code.

## Two factory overloads per family mirror the rebindable/plain split

`AddInput`/`AddInputVector`/`AddInputAxis` each have two lower-level, factory-taking overloads
beneath the action/name-taking one: one accepting a `Func<IServiceProvider, IInput>`-shaped
factory (registering only the plain interface, no alias - a factory that only promises the plain
interface has nothing rebindable to alias), and one accepting the rebindable-shaped factory
(registering the rebindable interface plus the plain-interface alias, exactly like the built-in
registration does). Overload resolution picks between them the way it would for any other
overloaded method: a factory lambda whose inferred return type implements the rebindable
interface binds to that overload, the more specific target, so a custom rebindable adapter gets
the alias without the caller having to pick the overload explicitly. A project with its own input
adapter calls one of these two directly, rather than reaching for the lower-level
`AddKeyedSingleton` and reimplementing the aliasing dance by hand.

## Rebinding is always erase-then-add, never merge

Every `Rebind*` method calls `InputMap.ActionEraseEvents` before `InputMap.ActionAddEvent`,
replacing every existing binding for that action with exactly the one given, rather than adding
an alternate binding alongside the old one. This matches how a typical "press a key to rebind"
settings screen behaves - the player's new key press is meant to *replace* their old choice, not
stack with it. A project that wants multiple simultaneous bindings for one action (e.g. both a
keyboard key and a gamepad button bound at once) can't get that from `Rebind*` - each call erases
whatever the previous call added - and would need to call `InputMap.ActionAddEvent` directly
instead, since that falls outside what a single-choice rebind UI needs.
