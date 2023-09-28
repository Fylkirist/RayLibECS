
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
        var cameraComponents = _world.GetComponents(_currentCamera.Id);
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

    public override void Update(long delta)
    {
        
    }

    public override void Detach()
    {
        _active = false;
    }

    private void InitializeNewCamera(){
        var initCam = _world.CreateEntity("camera");
        var collisionShape = (BoxCollisionShape)_world.CreateComponent(typeof(BoxCollisionShape));
        collisionShape.BoundingBox = new BoundingBox(
            new Vector3(0,0,0),
            new Vector3(1,1,1)
        );
        _world.AttachComponent(initCam,collisionShape);
        var cameraComp = (Camera3) _world.CreateComponent(typeof(Camera3));
        cameraComp.CameraMode = CameraMode.CAMERA_THIRD_PERSON;
        _world.AttachComponent(initCam,cameraComp);
    }
}