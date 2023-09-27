using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class StaticPosition:Component
{
    public Vector2 Position { get; set; }
    public StaticPosition(Entity owner,Vector2 position) : base(owner)
    {
        Position = position;
    }
}