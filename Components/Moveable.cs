namespace RayLibECS.Components;

public class Moveable : Component
{
    public bool IsMovable;

    public Moveable()
    {
        IsMovable = true;
    }
}