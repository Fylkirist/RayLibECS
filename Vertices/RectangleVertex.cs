using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Vertices;

internal class RectangleVertex : Vertex2D
{
    public Rectangle Vertex;
    public float Rotation;

    public RectangleVertex(Rectangle vertex, int rotation, Vector2 offset):base(offset)
    {
        Vertex = vertex;
        Rotation = rotation;
    }
}