using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Entities;
using RayLibECS.Shapes;

namespace RayLibECS.Components;

internal class CollisionEvent : Component
{
    public Entity? Collider;
    public Geometry2D[] Vertices;

    public CollisionEvent()
    {
        Collider = null;
        Vertices = new Geometry2D[2];
    }
}