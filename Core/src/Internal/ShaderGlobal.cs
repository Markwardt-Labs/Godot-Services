namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes a global shader
/// parameter via the engine's static <see cref="RenderingServer"/> API, which cannot be
/// substituted in a test.
/// </summary>
/// <typeparam name="T">The type of the shader global's value.</typeparam>
[ExcludeFromCodeCoverage]
internal sealed class ShaderGlobal<[MustBeVariant] T> : IMutableSetting<T>
{
    private readonly StringName name;

    /// <param name="name">The name of the global shader parameter.</param>
    internal ShaderGlobal(string name) => this.name = name;

    /// <inheritdoc />
    public T Get() => RenderingServer.GlobalShaderParameterGet(name).As<T>();

    /// <inheritdoc />
    public void Set(T value) => RenderingServer.GlobalShaderParameterSet(name, Variant.From(value));
}
