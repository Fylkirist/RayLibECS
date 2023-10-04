using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;

namespace RayLibECS.Systems;

internal class PhysicsSystem2D : System
{
    private float _gravity;
    private bool _active;
    public PhysicsSystem2D(World world) : base(world)
    {
        _gravity = 9.82f;
        _active = false;
    }

    public override void Initialize()
    {
        _active = true;
    }

    public override void Draw()
    {
        
    }

    public override void Update(float delta, InputState input)
    {
        if (!_active) return; 
        HandleCollisions(delta);
        HandleGravity(delta);
        HandleAcceleration(delta);
    }
    public override void Detach()
    {
        
    }

    private void HandleAcceleration(float delta)
    {
        foreach (var position in World.GetComponents<Position2>())
        {
            position.Position.Y += position.Speed.Y * delta;
            position.Position.X += position.Speed.X * delta;
        }
    }

    private void HandleCollisions(float delta)
    {
        var collisions = World.GetComponents<CollisionEvent>().ToArray();
        foreach (var collisionEvent in collisions)
        {
            
        }

    }

    private void HandleGravity(float delta)
    {
        var massiveEntities = World.GetEntitiesWith<Mass>();
        foreach (var entity in massiveEntities)
        {
            var mass = entity.Components.OfType<Mass>().FirstOrDefault();
            var position = entity.Components.OfType<Position2>().FirstOrDefault();
            if (mass == null || position == null)
            {
                continue;
            }
            position.Speed.Y += _gravity * delta;
        }
    }
}