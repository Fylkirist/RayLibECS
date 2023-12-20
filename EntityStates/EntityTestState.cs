using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Events;
using RayLibECS.Interfaces;
using System.Numerics;

namespace RayLibECS.EntityStates;

internal class EntityTestState : EntityStateBase
{
    public override void Update(World world, Entity entity, InputState input, float delta)
    {
        var physics = world.QueryComponent<Physics2>(entity);
        if (physics == null)
        {
            return;
        }
        
        physics.Velocity -= physics.Velocity * 0.9f * delta;
        physics.Velocity = physics.Velocity.Length() < 20? Vector2.Zero: physics.Velocity;
        
        foreach (var pressed in input.PressedKeys)
        {
            switch (pressed)
            {
                case KeyboardKey.KEY_RIGHT:
                    physics.Velocity.X = 150;
                    break;
                case KeyboardKey.KEY_LEFT:
                    physics.Velocity.X = -150;
                    break;
            }
        } 
    }
}
