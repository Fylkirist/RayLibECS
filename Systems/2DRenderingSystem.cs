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
        var camComponents = World.GetComponents(_currentCamera);
        foreach (var component in camComponents)
        {
            switch (component)
            {
                case Camera2 camera2: 

                    break;
                case StaticPosition staticPos:
                    
                    break;
            }
        }
        Raylib.DrawRectangle(300,300,300,300,Color.BLUE);
    }

    public override void Update(long delta, InputState input)
    {
        if(!_active) return;
        var camComponents = World.GetComponents(_currentCamera);
        foreach (var component in camComponents)
        {
            switch (component)
            {
                case Camera2 cam2:

                    break;
            }
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

        var camPosition = World.CreateComponent(typeof(Camera2)) as Camera2;
        camPosition.Position = new Camera2D(new Vector2(Raylib.GetScreenWidth()/2,Raylib.GetScreenHeight()/2), new Vector2(0,0), 0, 1);
        World.AttachComponent(newCam,camPosition);
        Raylib.BeginMode2D(camPosition.Position);
    }

    private void CleanupCameraEntity()
    {
        World.DestroyEntity(_currentCamera);
    }

    public RenderingSystem2D(World world) : base(world)
    {
        _currentCamera = new Entity(0, "");
        _active = false;
    }
}