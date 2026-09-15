namespace Markwardt.GodotServices.Tests;

/// <summary>
/// Creates natively-backed Godot test instances without running the engine.
/// </summary>
internal static class GodotObjects
{
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> without invoking its constructor.
    /// </summary>
    /// <remarks>
    /// A real <c>new T()</c> crashes the process outside a running Godot engine for any type with
    /// native interop backing it - not just a <see cref="GodotObject"/> subclass like
    /// <see cref="Node"/>/<see cref="Resource"/>, but also a plain-<see cref="object"/>-derived
    /// wrapper like <see cref="StringName"/>. Either kind's constructor calls into native code to
    /// allocate or intern its engine-side counterpart, which segfaults when no engine is hosting
    /// the process (as here, under plain <c>dotnet test</c>).
    /// <see cref="RuntimeHelpers.GetUninitializedObject"/> allocates the managed object directly,
    /// skipping every constructor in the chain, so no native call ever runs. The result is safe to
    /// use for plain reflection (get/set a C# property, read its <see cref="Type"/>) or to pass as
    /// an otherwise-unused argument, but must never have an actual engine API invoked on it (e.g.
    /// <c>Node.AddChild</c>, <c>Node.QueueFree</c>, <c>PackedScene.Instantiate</c>) - those still
    /// reach into the native object this trick never allocated. See <c>Docs/Testing.md</c>.
    /// </remarks>
    /// <typeparam name="T">The natively-backed type to create.</typeparam>
    /// <returns>An uninitialized instance of <typeparamref name="T"/>.</returns>
    internal static T CreateUninitialized<T>()
        where T : class
        => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
}
