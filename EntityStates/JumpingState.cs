using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using System.Numerics;
using Raylib_cs;
using RayLibECS.Events;

namespace RayLibECS.EntityStates;

public class JumpingState : EntityStateBase
{
    public override void Update(World world, Entity entity, InputState input, float delta)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        var charState = world.QueryComponent<EntityState>(entity);
        if(charPhysics == null || charStats == null || charState == null){
            return;
        }

        charPhysics.Velocity.Y += 100*9.81f*delta;
        
        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X + 100 * delta : charStats.Speed;
            charState.PushNextState("running");
        }
        else if(input.PressedKeys.Contains(KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X - 100 * delta : -charStats.Speed;
            charState.PushNextState("running");
        }

        foreach (var collision in world.GetWorldEvents<CollisionEvent2>())
        {
            if (collision.Collider1 == entity || collision.Collider2 == entity)
            {
                charState.PushNextState("grounded");
                break;
            }
        }
    }
}
