using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Components;

internal class DrawableCircle:RenderableComponent
{
    public Vector2 Position;
    public float Radius;
    public Color Colour;

    public DrawableCircle()
    {
        Position = new Vector2();
        Radius = 0;
        RenderingMode = RenderingModes.TwoD;
        Colour = Color.BLANK;
    }
}