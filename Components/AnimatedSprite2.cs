using System.Numerics;
using Raylib_cs;

namespace RayLibECS.Components;

public struct AnimatedSprite2
{
    public int Owner;
    public Vector2 Offset;
    public float Scale;
    public string AnimationState;
    public Dictionary<string,Texture2D> TextureStateMap;
    public Color Tint;
    public RenderingModes RenderingMode;
    public int Z;

    public AnimatedSprite2()
    {
        Owner = 0;
        Offset = new Vector2();
        Scale = 0f;
        AnimationState = "";
        TextureStateMap = new Dictionary<string,Texture2D>();
        Tint = Color.BLANK;
        RenderingMode = RenderingModes.TwoD;
        Z = 100;
    }
}
