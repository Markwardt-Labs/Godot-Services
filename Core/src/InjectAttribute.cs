namespace Markwardt.GodotServices;

/// <summary>
/// Marks a public property to be populated with a service resolved from the application's
/// dependency injection container when the declaring node enters the scene tree.
/// </summary>
/// <remarks>
/// Only public properties are supported, so that types using property injection remain fully
/// constructible and settable from unit tests without requiring the DI container. Declare an
/// injected property as <see langword="required"/> with an <see langword="init"/> setter (e.g.
/// <c>public required IThing Thing { get; init; }</c>) rather than a plain settable property
/// defaulted to <c>null!</c>: <see langword="required"/> needs no such placeholder default, and
/// reflection - what actually performs the injection - can still set an <see langword="init"/>
/// property after construction, since <see langword="init"/> and <see langword="required"/> are
/// both enforced by the compiler at ordinary C# construction sites only, not by the runtime. See
/// <see cref="ServiceContainer"/> for the composition root that performs the injection.
/// </remarks>
/// <param name="key">
/// The key identifying which keyed service registration to resolve, or <see langword="null"/> to
/// resolve the default, unkeyed registration.
/// </param>
[AttributeUsage(AttributeTargets.Property)]
public sealed class InjectAttribute(object? key = null) : Attribute
{
    /// <summary>
    /// The key identifying which keyed service registration to resolve, or <see langword="null"/>
    /// to resolve the default, unkeyed registration.
    /// </summary>
    public object? Key { get; } = key;
}
