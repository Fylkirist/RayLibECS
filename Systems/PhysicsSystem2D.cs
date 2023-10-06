using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Vertices;

namespace RayLibECS.Systems;

internal class PhysicsSystem2D : System
{
    private float _gravity;
    private bool _active;
    private float _scale;
    public PhysicsSystem2D(World world) : base(world)
    {
        _gravity = 9.82f;
        _active = false;
        _scale = 100;
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

    public override void Stop()
    {
        _active = false;
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
        var collisions = World.GetComponents<CollisionEvent>();
        foreach (var collisionEvent in collisions)
        {
            var ownerEntity = collisionEvent.Owner;
            var colidee = collisionEvent.Collider;
            var velocityDelta = (Vector2)CalculateCollisionPhysics(
                collisionEvent.Vertices[0].GetShapeAsType(),
                ownerEntity,
                collisionEvent.Vertices[1].GetShapeAsType(),
                colidee);
        }
    }

    private Vector2 CalculateCollisionPhysics(TriangleVertex triangle1, Entity colliderEntity, TriangleVertex triangle2, Entity collideEntity)
    {
        
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(TriangleVertex triangle, Entity colliderEntity, CircleVertex circle, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(TriangleVertex triangle, Entity colliderEntity, RectangleVertex rectangle, Entity collideEntity)
    {
        return Vector2.Zero;
    }

    private Vector2 CalculateCollisionPhysics(RectangleVertex rectangle1, Entity colliderEntity, RectangleVertex rectangle2, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(RectangleVertex rectangle, Entity colliderEntity, CircleVertex circle, Entity collideEntity)
    {
        return Vector2.Zero;
    }

    private Vector2 CalculateCollisionPhysics(RectangleVertex rectangle, Entity colliderEntity, TriangleVertex triangle,
        Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleVertex circle1, Entity colliderEntity, CircleVertex circle2, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleVertex circle, Entity colliderEntity,RectangleVertex rectangle, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleVertex circle, Entity colliderEntity, TriangleVertex triangle, Entity collideEntity)
    {
        return Vector2.Zero;
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
            position.Speed.Y += _gravity * _scale * delta;
        }
    }
}


public enum PhysicsType2D
{
    Kinematic,
    Dynamic,
    Static
}