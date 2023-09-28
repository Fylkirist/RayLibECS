using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components
{
    internal class SphereCollisionShape : Component
    {
        public float Radius { get; set; }
        public SphereCollisionShape(Entity owner) : base(owner)
        {
            Radius = 0;
        }
    }
}
