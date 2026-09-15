namespace Markwardt.GodotServices;

/// <summary>
/// Changes the active scene.
/// </summary>
public interface ISceneNavigator
{
    /// <summary>
    /// Changes the active scene to <paramref name="scene"/>.
    /// </summary>
    /// <param name="scene">The packed scene to change to.</param>
    void ChangeScene(PackedScene scene);
}
