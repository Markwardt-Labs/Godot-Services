namespace Markwardt.GodotServices;

/// <summary>
/// <inheritdoc cref="ISetting{T}" path="/summary"/> Also allows the setting's value to be
/// changed at runtime.
/// </summary>
/// <typeparam name="T">The type of the setting's value.</typeparam>
public interface IMutableSetting<T> : ISetting<T>
{
    /// <summary>
    /// Sets the setting's value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    void Set(T value);
}
