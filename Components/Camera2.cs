using Raylib_cs;

namespace RayLibECS.Components;

internal struct Camera2
{
    public int Owner;
    public Camera2D Position;
    public Camera2(int owner)
    {
        Position = new Camera2D();
        Owner = owner;
    }

    public Camera2()
    {
        Position = new Camera2D();
    }
}