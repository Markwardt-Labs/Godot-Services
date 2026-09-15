namespace Markwardt.GodotServices;

/// <summary>
/// Casts rays against a 3D physics world.
/// </summary>
public interface IRaycaster
{
    /// <summary>
    /// Casts a ray from <paramref name="from"/> to <paramref name="to"/> within
    /// <paramref name="world"/>, excluding the physics bodies in <paramref name="exclude"/>.
    /// </summary>
    /// <param name="world">The 3D world to cast the ray within.</param>
    /// <param name="from">The ray's start position.</param>
    /// <param name="to">The ray's end position.</param>
    /// <param name="exclude">The physics body RIDs to exclude from the cast.</param>
    /// <returns>The world-space position the ray hit, or <see langword="null"/> if it hit nothing.</returns>
    Vector3? Cast(World3D world, Vector3 from, Vector3 to, IReadOnlyList<Rid> exclude);
}
