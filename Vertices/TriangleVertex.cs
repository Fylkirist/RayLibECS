using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Vertices;

public class TriangleVertex : Vertex2D
{
    public Vector2[] Points;
    public float Rotation;

    public TriangleVertex(Vector2[] points, Vector2 offset, float rotation):base(offset)
    {
        Points = points;
        Rotation = rotation;
    }


    public override bool CollidesWith(CollisionDetectionSystem2D system, Position2 pos1, Vertex2D collider, Position2 pos2)
    {
        return collider switch
        {
            CircleVertex circle => system.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleVertex rectangle => system.DetectVertexCollision(this, pos1, rectangle, pos2),
            TriangleVertex triangle => system.DetectVertexCollision(this, pos1, triangle, pos2),
            _ => false
        };
    }
}