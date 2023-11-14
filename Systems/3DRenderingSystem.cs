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
        var renderables = SortAndPruneRenderables(World.GetComponents<RenderableComponent3>().ToArray(),cameraComponent);
     
        foreach (var renderable in renderables){
            var physics = World.QueryComponent<Physics3>(renderable.Owner);
            if(physics == null){
                continue;
            }
            Raylib.DrawMesh(renderable.Mesh.Mesh, renderable.Material, Matrix4x4.CreateTranslation(physics.Position+renderable.Mesh.Offset));
        }
    }

    private IEnumerable<RenderableComponent3> SortAndPruneRenderables(RenderableComponent3[] renderables, Camera3 component)
    {
        double angle = Math.Cos(component.CameraPosition.fovy / 57.29578);
        Vector3 direction = Vector3.Normalize(component.CameraPosition.target);

        var sorted = renderables.OrderBy(e=>
            World.QueryComponent<Physics3>(e.Owner)!.Position - component.CameraPosition.position
                ).ToArray();

        foreach (var renderable in sorted){
            var position = World.QueryComponent<Physics3>(renderable.Owner);
            if(position == null){
                continue;
            }
            Vector3 relativePosition = Vector3.Normalize(position.Position - component.CameraPosition.position);
            float product = Vector3.Dot(direction, relativePosition);
            if(product > angle){
                yield return renderable;
            }
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
