using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Interfaces;

namespace RayLibECS.Vertices;

internal class RectangleVertex : IVertex
{
    public Rectangle Vertex;
    public float Rotation;
    public Vector2 Offset;

    public RectangleVertex(Rectangle vertex, int rotation, Vector2 offset)
    {
        Vertex = vertex;
        Rotation = rotation;
        Offset = offset;
    }
}