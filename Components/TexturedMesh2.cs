using Raylib_cs;
using RayLibECS.Shapes;
using System.Numerics;
using RayLibECS.Systems;

namespace RayLibECS.Components;

public class TexturedMesh2 : RenderableComponent2{
    public Texture2D Texture;
    public Shape2D[] Shapes;
    public Vector2 Offset;
    public Shader Shader;
    public Texture2DisplayType TextureDisplay;

    public TexturedMesh2()
    {
        Shader = new Shader();
        TextureDisplay = Texture2DisplayType.Stretch;
        Texture = new Texture2D();
        Shapes = new Shape2D[0];
        Offset = Vector2.Zero;
    }
}
