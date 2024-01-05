using Raylib_cs;
using RayLibECS.Systems;

namespace RayLibECS.Components;

class MappedTexture2 : RenderableComponent2{
    public Texture2D Texture;
    public Shader Shader;
    public Texture2DisplayType TextureDisplay;

    public MappedTexture2()
    {
        TextureDisplay = Texture2DisplayType.Stretch;
        Texture = new Texture2D();
        Shader = new Shader();
    }
}
