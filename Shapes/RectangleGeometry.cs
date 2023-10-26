using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public class RectangleGeometry : Geometry2D
{
    public Vector2[] Vertices = new Vector2[4];
    public Rectangle Vertex;
    public float Rotation;

    public RectangleGeometry(Rectangle vertex, int rotation, Vector2 offset):base(offset)
    {
        Vertex = vertex;
        Rotation = rotation;
    }

    public Vector2[] GetRectPoints()
    {
        var points = new Vector2[4];
        points[0] = new Vector2(Vertex.x-Vertex.width/2, Vertex.y-Vertex.height);
        points[1] = new Vector2(Vertex.x+Vertex.width, Vertex.y);
        points[2] = new Vector2(Vertex.x+Vertex.width, Vertex.y+Vertex.height);
        points[3] = new Vector2(Vertex.x, Vertex.y+Vertex.height);
        for (int i = 0;i<4;i++)
        {
            points[i] = CollisionDetectionSystem2D.ApplyRotationMatrix(points[i], Vector2.Zero, Rotation);
        }
        return points;
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

    public override dynamic GetShapeAsType()
    {
        return this;
    }
}