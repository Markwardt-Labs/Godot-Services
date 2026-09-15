namespace Markwardt.GodotServices.Internal;

/// <summary>
/// <inheritdoc cref="IRaycaster" path="/summary"/> Wraps
/// <see cref="PhysicsDirectSpaceState3D.IntersectRay"/>, which cannot be substituted in a test.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class Raycaster : IRaycaster
{
    /// <inheritdoc />
    public Vector3? Cast(World3D world, Vector3 from, Vector3 to, IReadOnlyList<Rid> exclude)
    {
        PhysicsRayQueryParameters3D query = new()
        {
            From = from,
            To = to,
            Exclude = new Godot.Collections.Array<Rid>(exclude),
        };

        Godot.Collections.Dictionary result = world.DirectSpaceState.IntersectRay(query);
        return result.Count > 0 ? (Vector3)result["position"] : null;
    }
}
