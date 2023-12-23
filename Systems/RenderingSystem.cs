using Raylib_cs;
using RayLibECS.Components;
using System.Numerics;

namespace RayLibECS.Systems;

public class RenderingSystem : SystemBase
{
    private bool _running;

    public RenderingSystem(World world) : base(world)
    {
        _running = false;
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Draw()
    {
        Render3d();
        Render2d();
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
        foreach (var sprite2 in World.GetComponents<AnimatedSprite2>())
        {
            sprite2.UpdateAnimationTimer(delta);
        }
    }

    private void Render2d(){
        var camera2 = World.GetComponents<Camera2>().First();
        Raylib.BeginMode2D(camera2.Position);
    }

    private void RenderSprite2()
    {
        foreach (var sprite2 in World.GetComponents<AnimatedSprite2>())
        {
            var transform = World.QueryComponent<Physics2>(sprite2.Owner);
            if(transform == null) continue;
            Raylib.DrawTexturePro(World.AssetManager.GetTexture(sprite2.AnimationState),new Rectangle(),new Rectangle(), Vector2.Zero, sprite2.rotation, sprite2.Tint);
        }
    }

    private void Render3d(){

    }
}
