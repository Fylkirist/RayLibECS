using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Position3:Component
{
    public Vector3 Speed { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Direction { get; set; }
    public Position3(Entity owner,Vector3 position, Quaternion direction, Vector3 speed) : base(owner)
    {
        Position = position;
        Direction = direction;
        Speed = speed;
    }
}