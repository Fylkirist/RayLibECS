using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Vertices;

internal class TriangleVertex : Vertex2D
{
    public Vector2[] Points;
    public float Rotation;

    public TriangleVertex(Vector2[] points, Vector2 offset, float rotation):base(offset)
    {
        Points = points;
        Rotation = rotation;
    }
}