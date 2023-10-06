using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Camera2 : Component
{
    public Camera2D Position;
    public Camera2(Entity owner) : base(owner)
    {
        Position = new Camera2D();
    }

    public Camera2() : base()
    {
        Position = new Camera2D();
    }
}