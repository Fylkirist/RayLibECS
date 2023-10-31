using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal struct Position3
{
    public int Owner;
    public Vector3 Speed { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Direction { get; set; }
    public Position3(Vector3 position, Quaternion direction, Vector3 speed)
    {
        Owner = 0;
        Position = position;
        Direction = direction;
        Speed = speed;
    }
}