using RayLibECS.Entities;
using RayLibECS.Events;

namespace RayLibECS.Interfaces;

public interface IEntityState
{
    public void EnterState(World world, Entity entity);

    public void Update(World world, Entity entity, InputState input,float delta);

    public void ExitState(World world, Entity entity);
}
