using System.Numerics;
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
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
        {
            return;
        }
        
        var triangle1Center = Vector2.Transform(triangle1.Offset + physicsComponent1.Position, Matrix3x2.CreateRotation(physicsComponent1.Rotation, physicsComponent1.Position));
        var triangle2Center = Vector2.Transform(triangle2.Offset + physicsComponent2.Position, Matrix3x2.CreateRotation(physicsComponent2.Rotation, physicsComponent2.Position));

        var triangle1Transformed = new Vector2[]
        {
            Vector2.Transform(triangle1.Points[0]+triangle1Center,Matrix3x2.CreateRotation(triangle1.Rotation + physicsComponent1.Rotation,triangle1Center)),
            Vector2.Transform(triangle1.Points[1]+triangle1Center,Matrix3x2.CreateRotation(triangle1.Rotation + physicsComponent1.Rotation,triangle1Center)),
            Vector2.Transform(triangle1.Points[2]+triangle1Center,Matrix3x2.CreateRotation(triangle1.Rotation + physicsComponent1.Rotation,triangle1Center))
        };

        var triangle2Transformed = new Vector2[]
        {
            Vector2.Transform(triangle2.Points[0]+triangle2Center,Matrix3x2.CreateRotation(triangle2.Rotation + physicsComponent2.Rotation,triangle2Center)),
            Vector2.Transform(triangle2.Points[1]+triangle2Center,Matrix3x2.CreateRotation(triangle2.Rotation + physicsComponent2.Rotation,triangle2Center)),
            Vector2.Transform(triangle2.Points[2]+triangle2Center,Matrix3x2.CreateRotation(triangle2.Rotation + physicsComponent2.Rotation,triangle2Center))
        };

        var distance = triangle1Center - triangle2Center;
        var collisionNormal = Vector2.Normalize(distance);
        
        var relativeVelocity = physicsComponent1.Velocity - physicsComponent2.Velocity;
        var relativeSpeed = Vector2.Dot(relativeVelocity,collisionNormal);
        
        var impulse = -2 * relativeSpeed / (1 / physicsComponent1.Mass + 1 / physicsComponent2.Mass);

        physicsComponent1.Velocity += (impulse * collisionNormal) / physicsComponent1.Mass;
        physicsComponent2.Velocity -= (impulse * collisionNormal) / physicsComponent2.Mass;
        
        var radiusVector1 = new Vector2(triangle1Center.X - physicsComponent1.Position.X, triangle1Center.Y - physicsComponent1.Position.Y);
        var radiusVector2 = new Vector2(triangle2Center.X - physicsComponent2.Position.X, triangle2Center.Y - physicsComponent2.Position.Y);
        
        var angularImpulse1 = radiusVector1.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector1, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector1.LengthSquared():0;
        var angularImpulse2 = radiusVector2.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector2, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector2.LengthSquared():0;

        physicsComponent1.RotationSpeed += physicsComponent1.PhysicsType == PhysicsType2D.Dynamic ? angularImpulse1 / physicsComponent1.Mass : 0;
        physicsComponent2.RotationSpeed -= physicsComponent2.PhysicsType == PhysicsType2D.Dynamic ? angularImpulse2 / physicsComponent2.Mass : 0;

        float overlap = float.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            float projection1 = Vector2.Dot(triangle1Transformed[i], collisionNormal);
            float projection2 = Vector2.Dot(triangle2Transformed[i], collisionNormal);

            float currentOverlap = Math.Max(0, Math.Min(projection1, projection2) - Math.Max(projection1, projection2));
            overlap = Math.Min(overlap, currentOverlap);
        }

        Vector2 separationVector = overlap * collisionNormal;

        physicsComponent1.Position += separationVector * 0.5f;
        physicsComponent2.Position -= separationVector * 0.5f;

    }
    private void CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        CalculateCollisionPhysics(circle, collideEntity, triangle, colliderEntity);
    }
    private void CalculateCollisionPhysics(TriangleGeometry triangle, Entity colliderEntity, RectangleGeometry rectangle, Entity collideEntity)
    {
        CalculateCollisionPhysics(rectangle, collideEntity, triangle, colliderEntity);
    }

    private void CalculateCollisionPhysics(RectangleGeometry rectangle1, Entity colliderEntity, RectangleGeometry rectangle2, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
        {
            return;
        }
    }
    private void CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, CircleGeometry circle, Entity collideEntity)
    {
        CalculateCollisionPhysics(circle, collideEntity, rectangle, colliderEntity);
    }

    private void CalculateCollisionPhysics(RectangleGeometry rectangle, Entity colliderEntity, TriangleGeometry triangle, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
        {
            return;
        }
    }

    private void CalculateCollisionPhysics(CircleGeometry circle1, Entity colliderEntity, CircleGeometry circle2, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
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

        var radiusVector1 = new Vector2(circle1Center.X - physicsComponent1.Position.X, circle1Center.Y - physicsComponent1.Position.Y);
        var radiusVector2 = new Vector2(circle2Center.X - physicsComponent2.Position.X, circle2Center.Y - physicsComponent2.Position.Y);

        var angularImpulse1 = radiusVector1.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector1, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector1.LengthSquared(): 0;
        var angularImpulse2 = radiusVector2.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector2, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector2.LengthSquared(): 0;

        physicsComponent1.RotationSpeed += angularImpulse1 / physicsComponent1.Mass;
        physicsComponent2.RotationSpeed += angularImpulse2 / physicsComponent2.Mass;

    }
    private void CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity,RectangleGeometry rectangle, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);

        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
        {
            return;
        }

        var circleCenter = Vector2.Transform(physicsComponent1.Position + circle.Offset, Matrix3x2.CreateRotation(physicsComponent1.Rotation) * Matrix3x2.CreateTranslation(physicsComponent1.Position));
        var rectangleCenter = Vector2.Transform(physicsComponent2.Position + rectangle.Offset, Matrix3x2.CreateRotation(physicsComponent2.Rotation) * Matrix3x2.CreateTranslation(physicsComponent2.Position));

        var distance = circleCenter - rectangleCenter;
        distance = Vector2.Transform(distance, Matrix3x2.CreateRotation(-physicsComponent2.Rotation));

        var collisionNormal = Vector2.Normalize(distance);

        var relativeVelocity = physicsComponent1.Velocity - physicsComponent2.Velocity;
        var relativeSpeed = Vector2.Dot(relativeVelocity, collisionNormal);
        
        var clampedDistance = new Vector2(
            Math.Clamp(distance.X, -rectangle.WidthAndHeight().X / 2, rectangle.WidthAndHeight().X / 2),
            Math.Clamp(distance.Y, -rectangle.WidthAndHeight().Y / 2, rectangle.WidthAndHeight().Y / 2)
        );

        var impulse = -2 * relativeSpeed / (1 / physicsComponent1.Mass + 1 / physicsComponent2.Mass);
        var overlap = circle.Radius - clampedDistance.Length();
        overlap = Math.Max( overlap , 0 );
        var correction = overlap * collisionNormal;

        physicsComponent1.Position -= physicsComponent1.PhysicsType == PhysicsType2D.Dynamic ? correction : Vector2.Zero;
        physicsComponent2.Position += physicsComponent2.PhysicsType == PhysicsType2D.Dynamic ? correction : Vector2.Zero;

        var radiusVector1 = new Vector2(circleCenter.X - physicsComponent1.Position.X, circleCenter.Y - physicsComponent1.Position.Y);
        var radiusVector2 = new Vector2(clampedDistance.X, clampedDistance.Y);

        var angularImpulse1 = radiusVector1.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector1, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector1.LengthSquared():0;
        var angularImpulse2 = radiusVector2.LengthSquared() > 0 ? Vector3.Cross(new Vector3(radiusVector2, 0), new Vector3(collisionNormal, 0)).Z * impulse / radiusVector2.LengthSquared():0;

        physicsComponent1.Velocity += physicsComponent1.PhysicsType == PhysicsType2D.Dynamic ? impulse * collisionNormal / physicsComponent1.Mass : Vector2.Zero;
        physicsComponent2.Velocity -= physicsComponent2.PhysicsType == PhysicsType2D.Dynamic ? impulse * collisionNormal / physicsComponent2.Mass : Vector2.Zero;

        physicsComponent1.RotationSpeed += physicsComponent1.PhysicsType == PhysicsType2D.Dynamic ? angularImpulse1 / physicsComponent1.Mass : 0;
        physicsComponent2.RotationSpeed -= physicsComponent2.PhysicsType == PhysicsType2D.Dynamic ? angularImpulse2 / physicsComponent2.Mass : 0;

    }

    private void CalculateCollisionPhysics(CircleGeometry circle, Entity colliderEntity, TriangleGeometry triangle, Entity collideEntity)
    {
        var physicsComponent1 = World.QueryComponent<Physics2>(colliderEntity);
        var physicsComponent2 = World.QueryComponent<Physics2>(collideEntity);
        
        if (physicsComponent1 == null || physicsComponent2 == null || physicsComponent1.PhysicsType == PhysicsType2D.Ethereal || physicsComponent2.PhysicsType == PhysicsType2D.Ethereal)
        {
            return;
        }

        var circleCenter = Vector2.Transform(circle.Offset+physicsComponent1.Position,Matrix3x2.CreateRotation(physicsComponent1.Rotation,physicsComponent1.Position));
        var triangleCenter = Vector2.Transform(triangle.Offset+physicsComponent2.Position,Matrix3x2.CreateRotation(physicsComponent2.Rotation,physicsComponent2.Position));

        var triangleTransformed = new Vector2[]
        {
            Vector2.Transform(triangle.Points[0]+triangleCenter, Matrix3x2.CreateRotation(physicsComponent2.Rotation + triangle.Rotation,triangleCenter)),
            Vector2.Transform(triangle.Points[1]+triangleCenter, Matrix3x2.CreateRotation(physicsComponent2.Rotation + triangle.Rotation,triangleCenter)),
            Vector2.Transform(triangle.Points[2]+triangleCenter, Matrix3x2.CreateRotation(physicsComponent2.Rotation + triangle.Rotation,triangleCenter))
        };
        
        var distance = circleCenter - triangleCenter;
        var collisionNormal = Vector2.Normalize(distance);
        
        var relativeVelocity = physicsComponent1.Velocity - physicsComponent2.Velocity;
        var relativeSpeed = Vector2.Dot(relativeVelocity, collisionNormal);

        var impulse = -2 * relativeSpeed / (1 / physicsComponent1.Mass + 1 / physicsComponent2.Mass);

        physicsComponent1.Velocity += physicsComponent1.PhysicsType == PhysicsType2D.Dynamic ? impulse * collisionNormal / physicsComponent1.Mass : Vector2.Zero;
        physicsComponent2.Velocity -= physicsComponent2.PhysicsType == PhysicsType2D.Dynamic ? impulse * collisionNormal / physicsComponent2.Mass : Vector2.Zero;


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
