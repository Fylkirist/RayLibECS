using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using Raylib_cs;

namespace RayLibECS.EntityStates;

public class IdleState : IEntityState
{
    public void EnterState(World world, Entity entity)
    {
        var physicsComponent = world.QueryComponent<Physics2>(entity);
        var animSprite = world.QueryComponent<AnimatedSprite2>(entity);
        if(animSprite == null){
            throw new Exception($"Entity {entity.Id} must have sprite component");
        }
        if(physicsComponent == null){
            throw new Exception($"Entity {entity.Id} must have physics component");
        }
        animSprite.AnimationState = "idle";
        physicsComponent.PhysicsType = PhysicsType2D.Kinematic;
    }

    public void ExitState(World world, Entity entity)
    {
        
    }

    public void Update(World world, Entity entity, InputState input, float delta)
    {
        var charState = world.QueryComponent<EntityState>(entity);
        var physicsComponent = world.QueryComponent<Physics2>(entity);

        if(physicsComponent == null || charState == null){
            throw new Exception("Entity must contain physics component");
        }
        if(physicsComponent.Velocity.Length() > 0){
            physicsComponent.Velocity = physicsComponent.Velocity.Length() > 1? physicsComponent.Velocity * 0.1f * delta: Vector2.Zero;
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT) 
            || input.PressedKeys.Contains(KeyboardKey.KEY_LEFT))
        {
            charState.CurrentState = "running";
        }
    }
}
