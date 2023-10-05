using System.Numerics;
using Raylib_cs;
namespace RayLibECS.Components;

public class DrawableTriangle2D : RenderableComponent
{
    public Vector2[] Points;
    public Color Colour;
    public DrawableTriangle2D()
    {
        Points = new Vector2[]
        {
            new(),
            new(),
            new()
        };
        Colour = Color.WHITE;
    }
}