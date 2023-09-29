using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class DrawableRectangle : Component
{
    public Texture2D Texture;
    public Rectangle Rect;
    public DrawableRectangle(Entity owner) : base(owner)
    {
        Texture = new Texture2D();
        Rect = new Rectangle();
    }
}