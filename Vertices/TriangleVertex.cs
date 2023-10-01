using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Interfaces;

namespace RayLibECS.Vertices;

internal class TriangleVertex : IVertex
{
    public Vector2[] Points;
    public Vector2 Offset;
    public float Rotation;

    public TriangleVertex(Vector2[] points, Vector2 offset, float rotation)
    {
        Points = points;
        Offset = offset;
        Rotation = rotation;
    }
}