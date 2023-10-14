using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayLibECS.Systems;

namespace RayLibECS.Components;

internal class PhysicsType2 : Component
{
    public PhysicsType2D Type;
    public PhysicsType2()
    {
        Type = PhysicsType2D.Dynamic;
    }
}
