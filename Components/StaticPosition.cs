using System.Numerics;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal struct StaticPosition
{
    public int Owner;
    public Vector2 Position;
    public StaticPosition(int owner,Vector2 position)
    {
        Owner = owner;
        Position = position;
    }
}