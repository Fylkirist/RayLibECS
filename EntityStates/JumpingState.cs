using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;

namespace RayLibECS.EntityStates;

public class JumpingState : IEntityState
{
    public void EnterState(World world, Entity entity)
    {
        throw new NotImplementedException();
    }

    public void ExitState(World world, Entity entity)
    {
        throw new NotImplementedException();
    }

    public void Update(World world, Entity entity, InputState input, float delta)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        if(charPhysics == null){
            return;
        }
    }
}