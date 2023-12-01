using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using System.Numerics;
using Raylib_cs;

namespace RayLibECS.EntityStates;

public class JumpingState : IEntityState
{
    public void EnterState(World world, Entity entity)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var entityState = world.QueryComponent<EntityState>(entity);
        var charAnim = world.QueryComponent<AnimatedSprite2>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || entityState == null || charAnim == null || charStats == null){
            return;
        }
        
        charAnim.AnimationState = "jumping";
        charAnim.AnimationTimer = 0f;

        charPhysics.Velocity.Y -= charStats.JumpHeight;
        charPhysics.PhysicsType = PhysicsType2D.Kinematic;

        entityState.LastUpdate = "jumping";
    }

    public void ExitState(World world, Entity entity)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        if(charPhysics == null){
            return;
        }
        charPhysics.PhysicsType = PhysicsType2D.Kinematic; 
    }

    public void Update(World world, Entity entity, InputState input, float delta)
    {
        var anim = world.QueryComponent<AnimatedSprite2>(entity);
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || anim == null || charStats == null){
            return;
        }

        charPhysics.Velocity.Y += 100*9.81f*delta;
        
        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X + 100 * delta : charStats.Speed;
            anim.Facing = new Vector2(1,0);
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X - 100 * delta : -charStats.Speed;
            anim.Facing = new Vector2(-1,0);
        }
    }
}
