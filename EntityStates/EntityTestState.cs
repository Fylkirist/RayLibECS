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

namespace RayLibECS.EntityStates;

internal class EntityTestState : IEntityState
{
    public void EnterState(World world, Entity entity)
    {
        
    }

    public void Update(World world, Entity entity, InputState input, float delta)
    {
        var physics = world.QueryComponent<Physics2>(entity);
        if (physics == null)
        {
            return;
        }
        foreach (var pressed in input.PressedKeys)
        {
            switch (pressed)
            {
                case KeyboardKey.KEY_RIGHT:
                    physics.Position.X += 1000 * delta;
                    break;
                case KeyboardKey.KEY_LEFT:
                    physics.Position.X -= 1000 * delta;
                    break;
                case KeyboardKey.KEY_UP:
                    physics.Position.Y -= 1000 * delta;
                    break;
                case KeyboardKey.KEY_DOWN:
                    physics.Position.Y += 1000 * delta;
                    break;
            }
        }
    }

    public void ExitState(World world, Entity entity)
    {
        
    }
}