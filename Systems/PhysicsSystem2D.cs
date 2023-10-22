using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics;
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
    private double _simulationDistance;
    public PhysicsSystem2D(World world) : base(world)
    {
        _gravity = 9.82f;
        _active = false;
        _scale = 50;
        _simulationDistance = 2000;
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
            CalculateCollisionPhysics(
                collisionEvent.Vertices[0].GetShapeAsType(),
                ownerEntity,
                collisionEvent.Vertices[1].GetShapeAsType(),
                colidee);
        }
    }

    private void CalculateCollisionPhysics(TriangleGeometry triangle1, Entity colliderEntity, TriangleGeometry triangle2, Entity collideEntity)
    {
        
        
    }
    private void CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        
    }
    private void CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, RectangleGeometry rectangle, Entity collideEntity)
    {
        
    }

    private void CalculateCollisionPhysics(RectangleGeometry rectangle1, Entity colliderEntity, RectangleGeometry rectangle2, Entity collideEntity)
    {
        
    }
    private void CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        
    }

    private void CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, TriangleGeometry triangle,
        Entity collideEntity)
    {
        
    }
    private void CalculateCollisionPhysics(CircleGeometry circle1, Entity colliderEntity, CircleGeometry circle2, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType is PhysicsType2D.Ethereal)
        {
            return;
        }

        var circle1Center = Vector2.Transform(physicsComponent1.Position + circle1.Offset,
            Matrix3x2.CreateRotation(physicsComponent1.Rotation,physicsComponent1.Position));

        var circle2Center = Vector2.Transform(physicsComponent2.Position + circle2.Offset,
            Matrix3x2.CreateRotation(physicsComponent2.Rotation,physicsComponent2.Position));

        var collisionNormal = Vector2.Normalize(circle1Center - circle2Center);
        var relativeVelocity = physicsComponent1.Velocity - physicsComponent2.Velocity;
        var overlap = -((circle1Center - circle2Center).Length() - (circle1.Radius + circle2.Radius));
        if (overlap > 0)
        {
            var movement = (0.5f * collisionNormal) * overlap;
            physicsComponent1.Position += movement;
        }
        
        var relativeSpeed = Vector2.Dot(relativeVelocity, collisionNormal);

        var impulse = -2 * relativeSpeed / (1 / physicsComponent1.Mass + 1 / physicsComponent2.Mass);

        physicsComponent1.Velocity += impulse * collisionNormal / physicsComponent1.Mass * (1 - physicsComponent1.Absorbtion);
        physicsComponent2.Velocity -= impulse * collisionNormal / physicsComponent2.Mass * (1 - physicsComponent2.Absorbtion);

        var tangent = new Vector2(-collisionNormal.Y, collisionNormal.X);
        var frictionalImpulse = -Vector2.Dot(relativeVelocity, tangent) * (physicsComponent1.Friction + physicsComponent2.Friction);

        physicsComponent1.Velocity += frictionalImpulse * tangent / physicsComponent1.Mass;
        physicsComponent2.Velocity -= frictionalImpulse * tangent / physicsComponent2.Mass;

        var radiusVector1 = new Vector3(circle1Center.X - physicsComponent1.Position.X, circle1Center.Y - physicsComponent1.Position.Y, 0);
        var radiusVector2 = new Vector3(circle2Center.X - physicsComponent2.Position.X, circle2Center.Y - physicsComponent2.Position.Y, 0);

        physicsComponent1.RotationSpeed += (impulse * Vector3.Cross(radiusVector1, new Vector3(collisionNormal, 0)).Z / physicsComponent1.Mass);
        physicsComponent2.RotationSpeed -= (impulse * Vector3.Cross(radiusVector2, new Vector3(collisionNormal, 0)).Z / physicsComponent2.Mass);

    }
    private void CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity,RectangleGeometry rectangle, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType is PhysicsType2D.Ethereal)
        {
            return;
        }

        var circleCenter = Vector2.Transform(physicsComponent1.Position + circle.Offset,Matrix3x2.CreateRotation(physicsComponent1.Rotation,physicsComponent1.Position));
        var rectangleCenter = Vector2.Transform(physicsComponent2.Position + rectangle.Offset,Matrix3x2.CreateRotation(physicsComponent2.Rotation,physicsComponent2.Position));
        
        var distance = circleCenter - rectangleCenter;
        var collisionNormal = Vector2.Normalize(distance);

        var relativeVelocity = physicsComponent1.Velocity - physicsComponent2.Velocity;

        var relativeSpeed = Vector2.Dot(relativeVelocity,collisionNormal);

        var impulse = -2 * relativeSpeed / (1/physicsComponent1.Mass + 1/physicsComponent2.Mass);



    }
    private void CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity, TriangleGeometry triangle, Entity collideEntity)
    {
        
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
