namespace RayLibECS.Components;

public struct Moveable
{
    public int Owner;
    public bool IsMovable;

    public Moveable()
    {
        Owner = 0;
        IsMovable = true;
    }
}