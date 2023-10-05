using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

internal class RenderingSystem2D:System
{
    private Entity _currentCamera;
    private bool _active;
    public override void Initialize()
    {
        Initialize2DCamera();
        _active = true;
    }

    public override void Draw()
    {
        if(!_active) return;
        var activeCamera = GetActiveCamera();
        Raylib.BeginMode2D(activeCamera.Position);
        var renderables = World.GetComponents(RenderingModes.TwoD)
            .Cast<RenderableComponent>()
            .OrderBy(c => c.Z)
            .ToArray();
        foreach (var component in renderables)
        {
            var entity = component.Owner;
            var position = entity.Components.OfType<Position2>().FirstOrDefault();

            if (position == null)
                continue;
            
            switch (component)
            {
                case DrawableCircle circle:
                    Raylib.DrawCircle((int)position.Position.X, (int)position.Position.Y,circle.Radius,circle.Colour);
                    break;
                case DrawableRectangle rectangle:
                    Raylib.DrawRectangle((int)rectangle.Rect.x,(int)rectangle.Rect.y,(int)rectangle.Rect.width,(int)rectangle.Rect.height,rectangle.Colour);
                    break;
                case DrawableTriangle2D triangle:
                    Raylib.DrawTriangle(triangle.Points[0],triangle.Points[1],triangle.Points[2],triangle.Colour);
                    break;
            }
        }

        Raylib.EndMode2D();
    }

    public override void Update(float delta, InputState input)
    {
        if(!_active) return;
        var activeCam = GetActiveCamera();
        foreach (var key in input.PressedKeys)
        {
            if (key == KeyboardKey.KEY_RIGHT)
            {
                activeCam.Position.target.X += delta*100;
            }

            if (key == KeyboardKey.KEY_LEFT)
            {
                activeCam.Position.target.X -= delta*100;
            }

            if (key == KeyboardKey.KEY_UP)
            {
                activeCam.Position.target.Y -= delta*100;
            }

            if (key == KeyboardKey.KEY_DOWN)
                activeCam.Position.target.Y += delta*100;
        }
    }

    public override void Detach()
    {
        _active = false;
        CleanupCameraEntity();
        World.RemoveSystem(this);
    }

    private void Initialize2DCamera()
    {
        var newCam = World.CreateEntity("camera");
        _currentCamera = newCam;

        var camPosition = World.CreateComponent<Camera2>();
        camPosition.Position = new Camera2D(new Vector2(Raylib.GetScreenWidth()/2,Raylib.GetScreenHeight()/2), new Vector2(0,0), 0, 1);
        World.AttachComponent(newCam,camPosition);
    }

    private void CleanupCameraEntity()
    {
        World.DestroyEntity(_currentCamera);
    }

    private Camera2 GetActiveCamera()
    {
        var camera = World.GetComponents(_currentCamera).OfType<Camera2>().FirstOrDefault();
        return camera ?? new Camera2(_currentCamera);
    }

    public RenderingSystem2D(World world) : base(world)
    {
        _currentCamera = new Entity(0, "");
        _active = false;
    }
}