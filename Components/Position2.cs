using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Position2:Component
{
    public Vector2 Speed { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 Position { get; set; }
    public Position2(Entity owner,Vector2 position, Vector2 direction, Vector2 speed) : base(owner)
    {
        Position = position;
        Direction = direction;
        Speed = speed;
    }
}