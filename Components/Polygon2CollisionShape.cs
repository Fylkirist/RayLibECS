using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Polygon2CollisionShape:Component
{
    public List<Vector2> Points { get; set; }
    public Polygon2CollisionShape(Entity owner, List<Vector2> points):base(owner)
    {
        Points = points;
    }
}