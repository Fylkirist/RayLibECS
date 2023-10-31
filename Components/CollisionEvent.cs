using RayLibECS.Shapes;

namespace RayLibECS.Components;

internal struct CollisionEvent
{
    public int Owner;
    public int Collider;
    public Geometry2D[] Vertices;

    public CollisionEvent()
    {
        Owner = 0;
        Collider = 0;
        Vertices = new Geometry2D[2];
    }
}
