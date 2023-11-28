using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public class TriangleGeometry : Geometry2D
{
    public Vector2[] Points;
    public float Rotation;

    public TriangleGeometry(Vector2[] points, Vector2 offset, float rotation):base(offset)
    {
        Points = points;
        Rotation = rotation;
    }
}
