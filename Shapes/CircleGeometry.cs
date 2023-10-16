using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public class CircleGeometry : Geometry2D
{
    public float Radius;

    public CircleGeometry(float radius, Vector2 offset):base(offset)
    {
        Radius = radius;
    }

    public override bool CollidesWith(CollisionDetectionSystem2D systemBase, Physics2 pos1, Geometry2D collider, Physics2 pos2)
    {
        return collider switch
        {
            CircleGeometry circle => systemBase.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleGeometry rectangle => systemBase.DetectVertexCollision(rectangle, pos2, this, pos1),
            TriangleGeometry triangle => systemBase.DetectVertexCollision(triangle, pos2, this, pos1),
            _ => false
        };
    }

    public override dynamic GetShapeAsType()
    {
        return this;
    }
}