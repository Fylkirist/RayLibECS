using System.Numerics;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Interfaces;

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
        if(charPhysics == null || charState == null || anim == null){
            return;
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_RIGHT)){
            charPhysics.Velocity += 100 * delta * new Vector2(1,0);
            anim.Facing = new Vector2(1,0);
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_LEFT)){
            charPhysics.Velocity -= 100 * delta * new Vector2(1,0);
            anim.Facing = new Vector2(-1,0);
        }
        if(input.PressedKeys.Contains(Raylib_cs.KeyboardKey.KEY_SPACE)){
            charState.CurrentState = "jumping";
        }

    }
}
