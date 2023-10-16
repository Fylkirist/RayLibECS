using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;

internal class RenderingSystem2D:SystemBase
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
        var renderables = new List<RenderableComponent>();
        renderables.AddRange(World.GetComponents<ColouredMesh2>());
        foreach (var mesh in renderables)
        {
            switch (mesh)
            {
                case ColouredMesh2 mesh2:
                    RenderMesh(mesh2);
                    break;
            }
        }
    }

    public override void Update(float delta)
    {
        
        if(!_active) return;
        var activeCam = GetActiveCamera();
        foreach (var key in World.InputState.PressedKeys)
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

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    private void Initialize2DCamera()
    {
        var newCam = World.CreateEntity("camera");
        _currentCamera = newCam;

        var camPosition = World.CreateComponent<Camera2>();
        camPosition.Position = new Camera2D(new Vector2(Raylib.GetScreenWidth()/2,Raylib.GetScreenHeight()/2), new Vector2(0,0), 0, 1);
        World.AttachComponents(newCam,camPosition);
    }

    private void CleanupCameraEntity()
    {
        World.DestroyEntity(_currentCamera);
    }

    private Camera2 GetActiveCamera()
    {
        var camera = World.QueryComponent<Camera2>(_currentCamera);
        return camera ?? new Camera2(_currentCamera);
    }

    public RenderingSystem2D(World world) : base(world)
    {
        _currentCamera = new Entity(0, "");
        _active = false;
    }

    private void RenderMesh(ColouredMesh2 mesh)
    {
        var position = World.QueryComponent<Physics2>(mesh.Owner);
        if (position == null) return;
        for (int i = 0; i < mesh.Mesh.Shapes.Count; i++)
        {
            var shape = mesh.Mesh.Shapes[i];
            var colour = i < mesh.Colours.Count ? mesh.Colours[i] : mesh.Colours[^1];
            switch (shape)
            {
                case CircleGeometry circle:
                    var circleCenter = Vector2.Transform(position.Position + circle.Offset, Matrix3x2.CreateRotation(position.Rotation, position.Position));
                    Raylib.DrawCircle((int)circleCenter.X,(int)circleCenter.Y,circle.Radius,colour);
                    break;
                case RectangleGeometry rectangle: 
                    break;
                case TriangleGeometry triangle:
                    break;
            }
        }
    }
}