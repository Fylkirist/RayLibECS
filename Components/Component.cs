using RayLibECS.Entities;

namespace RayLibECS.Components;

internal abstract class Component
{
    public Entity Owner;

    protected Component(Entity owner)
    {
        Owner = owner;
    }
}