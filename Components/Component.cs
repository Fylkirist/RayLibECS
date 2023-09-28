using RayLibECS.Entities;

namespace RayLibECS.Components;

public abstract class Component
{
    public Entity Owner;

    protected Component(Entity owner)
    {
        Owner = owner;
    }
}