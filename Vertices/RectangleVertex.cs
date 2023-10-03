using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Vertices;

public class RectangleVertex : Vertex2D
{
    public Rectangle Vertex;
    public float Rotation;

    public RectangleVertex(Rectangle vertex, int rotation, Vector2 offset):base(offset)
    {
        Vertex = vertex;
        Rotation = rotation;
    }

    public Vector2[] GetRectPoints()
    {
        var points = new Vector2[4];
        points[0] = new Vector2(Vertex.x, Vertex.y);
        points[1] = new Vector2(Vertex.x+Vertex.width, Vertex.y);
        points[2] = new Vector2(Vertex.x+Vertex.width, Vertex.y+Vertex.height);
        points[3] = new Vector2(Vertex.x, Vertex.y+Vertex.height);
        for (int i = 0;i<4;i++)
        {
            points[i] = CollisionDetectionSystem2D.ApplyRotationMatrix(points[i], Vector2.Zero, Rotation);
        }
        return points;
    }


    public override bool CollidesWith(CollisionDetectionSystem2D system, Position2 pos1, Vertex2D collider, Position2 pos2)
    {
        return collider switch
        {
            CircleVertex circle => system.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleVertex rectangle => system.DetectVertexCollision(this, pos1, rectangle, pos2),
            TriangleVertex triangle => system.DetectVertexCollision(triangle, pos2, this, pos1),
            _ => false
        };
    }
}