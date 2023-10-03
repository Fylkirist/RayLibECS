using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Vertices;

public abstract class Vertex2D
{
    public Vector2 Offset;

    protected Vertex2D(Vector2 offset)
    {
        Offset = offset;
    }

    public abstract bool CollidesWith(CollisionDetectionSystem2D system, Position2 pos1, Vertex2D collider , Position2 pos2);
}