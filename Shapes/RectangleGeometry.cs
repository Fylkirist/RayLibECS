using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public class RectangleGeometry : Geometry2D
{
    public Vector2[] Vertices;
    public float Rotation;

    public RectangleGeometry(Rectangle vertex, int rotation, Vector2 offset):base(offset)
    {
        Rotation = rotation;
        Vertices = new Vector2[4]
        {
            new(-vertex.width/2,-vertex.height/2),
            new(vertex.width/2,-vertex.height/2),
            new(vertex.width/2,vertex.height/2),
            new(-vertex.width/2,vertex.height/2),
        };
    }

    public override bool CollidesWith(CollisionDetectionSystem2D systemBase, Physics2 pos1, Geometry2D collider, Physics2 pos2)
    {
        return collider switch
        {
            CircleGeometry circle => systemBase.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleGeometry rectangle => systemBase.DetectVertexCollision(this, pos1, rectangle, pos2),
            TriangleGeometry triangle => systemBase.DetectVertexCollision(triangle, pos2, this, pos1),
            _ => false
        };
    }

    public Vector2 WidthAndHeight()
    {
        return new Vector2(Vertices[2].X - Vertices[0].X, Vertices[3].Y - Vertices[0].Y);
    }

    public override dynamic GetShapeAsType()
    {
        return this;
    }
}