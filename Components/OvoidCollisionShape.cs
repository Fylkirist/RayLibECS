using RayLibECS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Components;

internal class OvoidCollisionShape : Component
{
    public float RadiusX { get; set; }
    public float RadiusY { get; set; }
    public float RadiusZ { get; set; }
    public OvoidCollisionShape(Entity owner) : base(owner)
    {
        RadiusX = 0;
        RadiusY = 0;
        RadiusZ = 0;
    }
}