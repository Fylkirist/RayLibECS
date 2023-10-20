using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Components;

public class AnimatedSprite2:RenderableComponent
{
    public Vector2 Offset;
    public float Scale;
    public string AnimationState;
    public Dictionary<string,Texture2D> TextureStateMap;
    public Color Tint;
  
    public AnimatedSprite2(){
        Offset = new Vector2();
        Scale = 0f;
        AnimationState = "";
        TextureStateMap = new Dictionary<string,Texture2D>();
        Tint = Color.BLANK;
    }
}
