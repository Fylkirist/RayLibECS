using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;

internal class PhysicsSystem2D : SystemBase
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

    public override void Update(float delta)
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
        foreach (var position in World.GetComponents<Physics2>())
        {
            position.Position.Y += position.Velocity.Y * delta;
            position.Position.X += position.Velocity.X * delta;
            position.Rotation += position.RotationSpeed * delta;
            if (position.Rotation > Math.PI * 2) position.Rotation -= (float)Math.PI * 2;
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

    private Vector2 CalculateCollisionPhysics(TriangleGeometry triangle1, Entity colliderEntity, TriangleGeometry triangle2, Entity collideEntity)
    {
        
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, RectangleGeometry rectangle, Entity collideEntity)
    {
        return Vector2.Zero;
    }

    private Vector2 CalculateCollisionPhysics(RectangleGeometry rectangle1, Entity colliderEntity, RectangleGeometry rectangle2, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        return Vector2.Zero;
    }

    private Vector2 CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, TriangleGeometry triangle,
        Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleGeometry circle1, Entity colliderEntity, CircleGeometry circle2, Entity collideEntity)
    {

        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity,RectangleGeometry rectangle, Entity collideEntity)
    {
        return Vector2.Zero;
    }
    private Vector2 CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity, TriangleGeometry triangle, Entity collideEntity)
    {
        return Vector2.Zero;
    }

    private void HandleGravity(float delta)
    {
        foreach (var physics in World.GetComponents<Physics2>())
        {
            if (physics.PhysicsType is not PhysicsType2D.Static and not PhysicsType2D.Kinematic)
            {
                physics.Velocity.Y += _gravity * _scale * delta;
            }
        }
    }
}


public enum PhysicsType2D
{
    Kinematic,
    Dynamic,
    Static,
    Ethereal
}
