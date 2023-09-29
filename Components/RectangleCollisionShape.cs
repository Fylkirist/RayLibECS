using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class RectangleCollisionShape : Component
{
    public Rectangle Shape;
    public RectangleCollisionShape(Entity owner) : base(owner)
    {
        Shape = new Rectangle();
    }
}