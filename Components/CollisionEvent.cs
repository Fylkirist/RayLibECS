using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Entities;
using RayLibECS.Vertices;

namespace RayLibECS.Components;

internal class CollisionEvent : Component
{
    public Entity? Collider;
    public Vertex2D[] Vertices;

    public CollisionEvent()
    {
        Collider = null;
        Vertices = new Vertex2D[2];
    }
}