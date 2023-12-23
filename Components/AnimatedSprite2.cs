using System.Numerics;
using Raylib_cs;

namespace RayLibECS.Components;

public class AnimatedSprite2 : RenderableComponent
{
    public float rotation;
    public Vector2 Offset;
    public float Scale;
    public string AnimationState;
    public float AnimationTimer;
    public Dictionary<string,Animation2D> TextureStateMap;
    public Color Tint;
    public Vector2 Facing;
  
    public AnimatedSprite2()
    {
        rotation = 0f;
        Facing = new Vector2();
        Offset = new Vector2();
        Scale = 0f;
        AnimationState = "";
        TextureStateMap = new Dictionary<string,Animation2D>();
        Tint = Color.BLANK;
        AnimationTimer = 0f;
    }

    public void UpdateAnimationTimer(float delta)
    {
        var nextFrameTime = AnimationTimer + delta;
        if(nextFrameTime > TextureStateMap[AnimationState].CycleLength)
        {
            nextFrameTime -= TextureStateMap[AnimationState].CycleLength;
        }
        AnimationTimer = nextFrameTime;
    }
}

public struct Animation2D
{
    public string Spritesheet;
    public AnimationFrame2D[] FrameData;
    public int Height;
    public int Width;
    public float CycleLength;

    public Animation2D(string spriteSheet, int frameHeight, int frameWidth, float animLength, AnimationFrame2D[] frames)
    {
        Spritesheet = spriteSheet;
        FrameData = frames;
        Height = frameHeight;
        Width = frameWidth;
        CycleLength = animLength;
    }
}

public struct AnimationFrame2D
{
    public float FrameTime;
    public int Offset;

    public AnimationFrame2D(float frameTime, int offset)
    {
        FrameTime = frameTime;
        Offset = offset;
    }
}
