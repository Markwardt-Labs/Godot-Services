namespace Markwardt.GodotServices.Internal;

/// <inheritdoc cref="IMutableSetting{T}" path="/summary"/> Reads/writes the active locale via the
/// engine's static <see cref="TranslationServer"/> API, which cannot be substituted in a test.
[ExcludeFromCodeCoverage]
internal sealed class Locale : IMutableSetting<string>
{
    /// <inheritdoc />
    public string Get() => TranslationServer.GetLocale();

    /// <inheritdoc />
    public void Set(string value) => TranslationServer.SetLocale(value);
}
