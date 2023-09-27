namespace RayLibECS.Components
public class Polygon3CollisionShape:Component
{
    public List<Vector2> Points;
    public Polygon2CollisionShape(Entity owner, List<Vector2> points):base(owner)
    {
        Points = points;
    }
}