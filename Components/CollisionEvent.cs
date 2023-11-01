using RayLibECS.Shapes;

namespace RayLibECS.Components;

internal struct CollisionEvent
{
    public int Owner;
    public int Collider;
    public Geometry2D[] Vertices;

    public CollisionEvent()
    {
        Owner = -1;
        Collider = -1;
        Vertices = new Geometry2D[2];
    }
}
