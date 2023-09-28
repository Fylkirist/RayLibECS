using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class BoxCollisionShape : Component
{
    public BoundingBox BoundingBox { get; set; }

    public BoxCollisionShape(Entity owner) : base(owner)
    {
        BoundingBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    }
}