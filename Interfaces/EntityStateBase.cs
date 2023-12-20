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

    public abstract void EnterState(World world, Entity entity);

    public abstract void Update(World world, Entity entity, InputState input,float delta);

    public abstract void ExitState(World world, Entity entity);
}
