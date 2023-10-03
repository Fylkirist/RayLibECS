
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

    public override void Update(float delta, InputState input)
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
        var initCam = World.CreateEntity("camera"); ;
        var cameraComp = World.CreateComponent<Camera3>();
        cameraComp.CameraMode = CameraMode.CAMERA_THIRD_PERSON;
        World.AttachComponent(initCam,cameraComp);
        _currentCamera = initCam;
    }

    private void CleanupCameraEntity()
    {
        World.DestroyEntity(_currentCamera);
    }
}