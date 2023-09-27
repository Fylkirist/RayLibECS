namespace RayLibECS.Components
public class Polygon3CollisionShape:Component
{
    public List<Vector3> Points;
    public Polygon3CollisionShape(Entity owner, List<Vector3> points):base(owner)
    {
        Points = points;
    }
}