namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="ISceneNavigator" path="/summary"/> Wraps the engine's global scene tree,
/// which cannot be substituted in a test.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class SceneNavigator : ISceneNavigator
{
    /// <inheritdoc />
    public void ChangeScene(PackedScene scene) => ((SceneTree)Engine.GetMainLoop()).ChangeSceneToPacked(scene);
}
