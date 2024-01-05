using Raylib_cs;
using RayLibECS.Components;
using System.Numerics;
using RayLibECS.Shapes;

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
        var renderables = World.GetRenderables2().OrderByDescending(e => e.Z);
        foreach(var renderable in renderables){
            switch(renderables){
                case AnimatedSprite2 sprite:
                    RenderSprite2(sprite);
                    break;
                
                case MappedTexture2 mappedTexture:
                    RenderMappedTexture2(mappedTexture);
                    break;
                
                case TexturedMesh2 texturedMesh:
                    RenderMesh2(texturedMesh);
                    break;
            }
        }
    }

    private void RenderMappedTexture2(MappedTexture2 mappedTexture)
    {
        return; 
    }

    private void RenderSprite2(AnimatedSprite2 sprite2)
    {
        var transform = World.QueryComponent<Physics2>(sprite2.Owner);
        if(transform == null) return;
        AnimationFrame2D previous = sprite2.TextureStateMap[sprite2.AnimationState].FrameData[0];
        foreach(var frameData in sprite2.TextureStateMap[sprite2.AnimationState].FrameData){
            if(frameData.FrameTime < sprite2.AnimationTimer){
                previous = frameData;
                continue;
            }
            Rectangle destinationRectangle = new Rectangle(
                    transform.Position.X - sprite2.TextureStateMap[sprite2.AnimationState].Width/2,
                    transform.Position.Y - sprite2.TextureStateMap[sprite2.AnimationState].Height/2,
                    sprite2.TextureStateMap[sprite2.AnimationState].Width,
                    sprite2.TextureStateMap[sprite2.AnimationState].Height
                );
            Raylib.DrawTexturePro(World.AssetManager.GetTexture(sprite2.AnimationState),previous.Source,destinationRectangle, transform.Position, sprite2.Rotation, sprite2.Tint);
            break;
        }
    }

    private void RenderMesh2(TexturedMesh2 mesh2)
    {
        var transform = World.QueryComponent<Physics2>(mesh2.Owner);
        if(transform == null) return;
        for(int i = 0; i<mesh2.Shapes.Length; i++){
            RenderShape2D(mesh2.Shapes[i], mesh2.Texture, transform);
        }
    }

    private void RenderShape2D(Shape2D shape, Texture2D texture, Physics2 transform)
    {
        switch(shape.Type){
            case ShapeType2D.Circle:
                
                break;

            case ShapeType2D.Triangle:
                break;

            case ShapeType2D.Polygon2:
                break;

            case ShapeType2D.Rectangle:
                break;

            case ShapeType2D.SymmetricalPolygon:
                break;

            default:
                break;
        }
    }

    private void RenderTexture2(){

    }

    private void Render3d(){

    }
}

public enum Texture2DisplayType{
    Tile,
    Stretch,
    PerShape
}
