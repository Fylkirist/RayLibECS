using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;

namespace RayLibECS.EntityStates;

public class RunningState : IEntityState
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
        var charState = world.QueryComponent<EntityState>(entity);
        if(charPhysics == null || charState == null){
            return;
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity += 100 * delta * new Vector2(1,0);
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity -= 100 * delta * new Vector2(1,0);
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_SPACE)){
            charState.CurrentState = "jump";
        }

    }
}
