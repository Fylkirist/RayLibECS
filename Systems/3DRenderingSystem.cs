
using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

public class RenderingSystem3D : System
{
    private Entity? _currentCamera;
    private bool _active;
    public RenderingSystem3D(World world) : base(world)
    {
        _active = false;
    }
    public override void Draw()
    {
        if (!_active) return;
        var cameraComponents = World.GetComponents(_currentCamera.Id);
        foreach (var component in cameraComponents)
        {
            switch (component.GetType())
            {
                default:
                    break;
            }
        }
    }

    public override void Initialize()
    {
        InitializeNewCamera();
        _active = true;
    }

    public override void Update(long delta, InputState input)
    {
        if(!_active) return;
        var cameraComponents = World.GetComponents(_currentCamera);
    }

    public override void Detach()
    {
        _active = false;
        CleanupCameraEntity();
    }

    private void InitializeNewCamera(){
        var initCam = World.CreateEntity("camera");
        var collisionShape = (BoxCollisionShape)World.CreateComponent(typeof(BoxCollisionShape));
        collisionShape.BoundingBox = new BoundingBox(
            new Vector3(0,0,0),
            new Vector3(1,1,1)
        );
        World.AttachComponent(initCam,collisionShape);
        var cameraComp = (Camera3) World.CreateComponent(typeof(Camera3));
        cameraComp.CameraMode = CameraMode.CAMERA_THIRD_PERSON;
        World.AttachComponent(initCam,cameraComp);
    }

    private void CleanupCameraEntity()
    {
        World.DestroyEntity(_currentCamera);
    }
}