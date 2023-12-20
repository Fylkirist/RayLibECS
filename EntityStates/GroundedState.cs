using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Events;
using RayLibECS.Interfaces;

namespace RayLibECS.EntityStates;

internal class GroundedState : EntityStateBase
{
    public override void Update(World world, Entity entity, InputState input, float delta)
    {
        var charPhysics = world.QueryComponent<Physics2>(entity);
        var charState = world.QueryComponent<EntityState>(entity);
        if (charPhysics == null || charState == null)
        {
            return;
        }

        charPhysics.Velocity.Y = 0;
        charPhysics.PhysicsType = PhysicsType2D.Kinematic;

        bool inContact = false;
        foreach (var collision in world.GetWorldEvents<CollisionEvent2>())
        {
            if (collision.Collider1 == entity || collision.Collider2 == entity)
            {
                inContact = true;
                break;
            }
        }

        if (!inContact)
        {
            charState.PushNextState("jumping");
        }
    }
}