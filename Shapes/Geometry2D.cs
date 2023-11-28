using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public abstract class Geometry2D
{
    public Vector2 Offset;
    public float Rotation;

    protected Geometry2D(Vector2 offset, float rotation)
    {
        Rotation = rotation;
        Offset = offset;
    }
}
