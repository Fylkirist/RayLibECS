using RayLibECS.Entities;

namespace RayLibECS.Interfaces;

public interface ICharacterState
{

    public abstract void EnterState(World world, Entity entity);

    public abstract void Update(World world, Entity entity);

    public abstract void ExitState(World world, Entity entity);
}
