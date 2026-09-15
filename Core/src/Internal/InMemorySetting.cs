namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Stores its value purely in memory,
/// starting at <paramref name="initialValue"/>, rather than reading/writing any engine API.
/// </summary>
/// <typeparam name="T">The type of the setting's value.</typeparam>
/// <param name="initialValue">The setting's initial value.</param>
internal sealed class InMemorySetting<T>(T initialValue) : IMutableSetting<T>
{
    private T value = initialValue;

    /// <inheritdoc />
    public T Get() => value;

    /// <inheritdoc />
    public void Set(T value) => this.value = value;
}
