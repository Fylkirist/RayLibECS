using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using Raylib_cs;

namespace RayLibECS.EntityStates;

public class RunningStateBase : EntityStateBase
{

    public override void EnterState(World world, Entity entity)
    {

    }


    public override void ExitState(World world, Entity entity)
    {
        
    }


    public override void Update(World world, Entity entity, InputState input, float delta)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charState = world.QueryComponent<EntityState>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || charState == null || charStats == null){
            return;
        }

        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X + 100 * delta : charStats.Speed;
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X - 100 * delta : -charStats.Speed;
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_SPACE)){
            charState.CurrentState = "jumping";
            return;
        }
        if (!input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT) && !input.PressedKeys.Contains(KeyboardKey.KEY_LEFT))
        {
            charState.CurrentState = "idle";
        }

    }
}
