using System.Numerics;
using Raylib_cs;

namespace RayLibECS.Components;

public class AnimatedSprite2 : RenderableComponent
{
    public Vector2 Offset;
    public float Scale;
    public string AnimationState;
    public float AnimationTimer;
    public Dictionary<string,KeyValuePair<float,string>[]> TextureStateMap;
    public Color Tint;
    public Vector2 Facing;
  
    public AnimatedSprite2(){
        Facing = new Vector2();
        Offset = new Vector2();
        Scale = 0f;
        AnimationState = "";
        TextureStateMap = new Dictionary<string,KeyValuePair<float,string>[]>();
        Tint = Color.BLANK;
        AnimationTimer = 0f;
    }
}
