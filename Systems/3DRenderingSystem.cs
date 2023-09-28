
using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

public class RenderingSystem3D : System
{
    private Entity? _currentCamera;
    private World _world;
    public RenderingSystem3D(World world){
        _world = world;
    }
    public override void Draw()
    {
        _world.GetComponents(_currentCamera.Id);
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override void Update(long delta)
    {
        
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    private void InitializeNewCamera(){
        var initCam = _world.CreateEntity("camera");
        _world.AttachComponent(initCam,(Polygon3CollisionShape)_world.CreateComponent(typeof(Polygon3CollisionShape)));
        _world.AttachComponent(initCam,(Position3)_world.CreateComponent(typeof(Position3)));
    }
}