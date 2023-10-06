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

public class CircleVertex : Vertex2D
{
    public Vector2 Center;
    public float Radius;

    public CircleVertex(Vector2 center, float radius, Vector2 offset):base(offset)
    {
        Center = center;
        Radius = radius;
    }

    public override bool CollidesWith(CollisionDetectionSystem2D system, Position2 pos1, Vertex2D collider, Position2 pos2)
    {
        return collider switch
        {
            CircleVertex circle => system.DetectVertexCollision(this, pos1, circle, pos2),
            RectangleVertex rectangle => system.DetectVertexCollision(rectangle, pos2, this, pos1),
            TriangleVertex triangle => system.DetectVertexCollision(triangle, pos2, this, pos1),
            _ => false
        };
    }

    public override dynamic GetShapeAsType()
    {
        return this;
    }
}