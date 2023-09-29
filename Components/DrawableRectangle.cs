using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class DrawableRectangle : RenderableComponent
{
    public Color Colour;
    public Rectangle Rect;
    public DrawableRectangle() : base()
    {
        Colour = Color.BLANK;
        Rect = new Rectangle();
    }
}