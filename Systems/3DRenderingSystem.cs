using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

public class RenderingSystem3D : SystemBase
{
    private long _renderDistance;
    private Entity? _currentCamera;
    private bool _active;
    public RenderingSystem3D(World world, long renderDistance) : base(world)
    {
        _renderDistance = renderDistance;
        _active = false;
    }
    public override void Draw()
    {
        if (!_active) return;
        if (_currentCamera == null) return;
        var cameraComponent = World.QueryComponent<Camera3>(_currentCamera);
        if(cameraComponent == null) return;
        var renderables = World.GetComponents<RenderableComponent3>().ToArray();
     
        foreach (var renderable in renderables){
            
        }
    }

    private IEnumerable<RenderableComponent3> SortAndPruneRenderables(RenderableComponent3[] renderables, Camera3 component)
    {
        double angle = Math.Cos(component.FieldOfView / 57.29578);

        var sorted = renderables.OrderBy(e=>
            World.QueryComponent<Physics3>(e.Owner).Position - component.CameraPosition
                ).ToArray();
        
        foreach(var renderable in sorted){
            yield renderable;
        }
    }
   

    public override void Initialize()
    {
        InitializeNewCamera();
        _active = true;
    }

    public override void Update(float delta)
    {
        if(!_active) return;
        if(_currentCamera == null) return;
        var cameraComponents = World.GetComponents(_currentCamera);
    }

    public override void Detach()
    {
        _active = false;
        CleanupCameraEntity();
    }

    public override void Stop()
    {
        _active = false;
    }

    private void InitializeNewCamera(){
        var initCam = World.CreateEntity("camera"); ;
        var cameraComp = World.CreateComponent<Camera3>();
        cameraComp.CameraMode = CameraMode.CAMERA_THIRD_PERSON;
        
        World.AttachComponents(initCam,cameraComp);
        _currentCamera = initCam;
    }

    private void CleanupCameraEntity()
    {
        if (_currentCamera != null)
        {
            World.DestroyEntity(_currentCamera);
        }
    }
}
