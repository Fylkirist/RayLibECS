using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Position2 : Component
{
    public Vector2 Speed;
    public float Rotation;
    public float RotationSpeed;
    public Vector2 Position;
    public Position2(Entity owner,Vector2 position, float rotation,float rotationSpeed, Vector2 speed) : base(owner)
    {
        Position = position;
        Rotation = rotation;
        RotationSpeed = rotationSpeed;
        Speed = speed;
    }

    public Position2()
    {
        Speed = Vector2.Zero;
        Rotation = 0f;
        Position = Vector2.Zero;
        RotationSpeed = 0f;
    }
}