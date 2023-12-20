using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;
using System.Numerics;
using Raylib_cs;
using RayLibECS.Events;

namespace RayLibECS.EntityStates;

public class JumpingStateBase : EntityStateBase
{
    public JumpingStateBase()
    {
        EventBus.Instance.Subscribe<CollisionEvent2>(LandOnSurface);
    }
    public override void EnterState(World world, Entity entity)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var entityState = world.QueryComponent<EntityState>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || entityState == null || charStats == null){
            return;
        }

        charPhysics.Velocity.Y -= charStats.JumpHeight;
        charPhysics.PhysicsType = PhysicsType2D.Kinematic;

        entityState.LastUpdate = "jumping";
    }

    public override void ExitState(World world, Entity entity)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        if(charPhysics == null){
            return;
        }
        charPhysics.PhysicsType = PhysicsType2D.Kinematic; 
    }

    public override void Update(World world, Entity entity, InputState input, float delta)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charStats = world.QueryComponent<CharacterStats>(entity);
        if(charPhysics == null || charStats == null){
            return;
        }

        charPhysics.Velocity.Y += 100*9.81f*delta;
        
        if(input.PressedKeys.Contains(KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X + 100 * delta : charStats.Speed;
        }
        if(input.PressedKeys.Contains(KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity.X = charPhysics.Velocity.X<charStats.Speed? charPhysics.Velocity.X - 100 * delta : -charStats.Speed;
        }
    }

    private void LandOnSurface(CollisionEvent2 collisionEvent)
    {
        var entity1 = collisionEvent.Collider1;
        var entity2 = collisionEvent.Collider2;

        if (World == null)
        {
            return;
        }
        var state1 = World.QueryComponent<EntityState>(entity1);
        var state2 = World.QueryComponent<EntityState>(entity2);

        if (state1 == null || state2 == null)
        {
            return;
        }

        if (state1.CurrentState == "jumping")
        {
            state1.CurrentState = "running";
        }

        if (state2.CurrentState == "jumping")
        {
            state2.CurrentState = "running";
        }
    }
}
