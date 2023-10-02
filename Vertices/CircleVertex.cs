using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Vertices;

internal class CircleVertex : Vertex2D
{
    public Vector2 Center;
    public float Radius;

    public CircleVertex(Vector2 center, float radius, Vector2 offset):base(offset)
    {
        Center = center;
        Radius = radius;
    }
}