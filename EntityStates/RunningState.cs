using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using Raylib_cs;

namespace RayLibECS.EntityStates;

public class RunningState : IEntityState
{
    public void EnterState(World world, Entity entity)
    {
        var anim = world.QueryComponent<AnimatedSprite2>(entity);
        if(anim==null){
            return;
        }
        anim.AnimationState = "running";
    }

    public void ExitState(World world, Entity entity)
    {
        
    }

    public void Update(World world, Entity entity, InputState input, float delta)
    {
        var anim = world.QueryComponent<AnimatedSprite2>(entity);
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charState = world.QueryComponent<EntityState>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || charState == null || anim == null || charStats == null){
            return;
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X + 100 * delta : charStats.Speed;
            anim.Facing = new Vector2(1,0);
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X - 100 * delta : -charStats.Speed;
            anim.Facing = new Vector2(-1,0);
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_SPACE)){
            charState.CurrentState = "jumping";
        }

    }
}
