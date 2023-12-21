using System.Numerics;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Interfaces;

public interface IBoundingRectable
{
    public Rectangle GetBoundingRect(Vector2 worldPosition);

    public Entity GetOwner();
}
