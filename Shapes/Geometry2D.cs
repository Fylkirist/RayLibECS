using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Systems;

namespace RayLibECS.Shapes;

public abstract class Geometry2D
{
    public Vector2 Offset;

    protected Geometry2D(Vector2 offset)
    {
        Offset = offset;
    }

    public abstract bool CollidesWith(CollisionDetectionSystem2D systemBase, Physics2 pos1, Geometry2D collider , Physics2 pos2);

    public abstract dynamic GetShapeAsType();
}