using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public class TriangleGeometry : Geometry2D
{
    public Vector2[] Points;
    public float Rotation;

    public TriangleGeometry(Vector2[] points, Vector2 offset, float rotation):base(offset)
    {
        Points = points;
        Rotation = rotation;
    }


    public override bool CollidesWith(CollisionDetectionSystem2D systemBase, Physics2 pos1, Geometry2D collider, Physics2 pos2)
    {
        return collider switch
        {
            CircleGeometry circle => systemBase.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleGeometry rectangle => systemBase.DetectVertexCollision(this, pos1, rectangle, pos2),
            TriangleGeometry triangle => systemBase.DetectVertexCollision(this, pos1, triangle, pos2),
            _ => false
        };
    }

    public override dynamic GetShapeAsType()
    {
        return this;
    }
}