namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes <paramref name="name"/>
/// via the engine's static <see cref="Godot.ProjectSettings"/> API, which cannot be substituted
/// in a test.
/// </summary>
/// <typeparam name="T">The type of the setting's value.</typeparam>
/// <param name="name">The name of the project setting.</param>
[ExcludeFromCodeCoverage]
internal sealed class ProjectSetting<[MustBeVariant] T>(string name) : IMutableSetting<T>
{
    /// <inheritdoc />
    public T Get() => Godot.ProjectSettings.GetSetting(name).As<T>();

    /// <inheritdoc />
    public void Set(T value) => Godot.ProjectSettings.SetSetting(name, Variant.From(value));
}
