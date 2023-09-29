using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class CollisionEvent : Component
{
    public Component? Collider { get; set; }

    public CollisionEvent(Entity owner) : base(owner)
    {
        Collider = null;
    }
}