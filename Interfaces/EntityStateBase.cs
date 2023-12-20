using RayLibECS.Entities;
using RayLibECS.Events;
using RayLibECS;

namespace RayLibECS.Interfaces;

public abstract class EntityStateBase
{
    protected World? World;

    protected EntityStateBase()
    {
        World = World.Instance;
    }

    public abstract void Update(World world, Entity entity, InputState input,float delta);

}
