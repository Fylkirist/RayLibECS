using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Vertices;

internal class Vertex2D
{
    public Vector2 Offset;

    public Vertex2D(Vector2 offset)
    {
        Offset = offset;
    }
}