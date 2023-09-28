
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
        throw new NotImplementedException();
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }

    private void InitializeNewCamera(){
        var initCam = _world.CreateEntity("camera");
        _world.AttachComponent(initCam,(Polygon3CollisionShape)_world.CreateComponent(typeof(Polygon3CollisionShape)));
        _world.AttachComponent(initCam,(Position3)_world.CreateComponent(typeof(Position3)));
    }
}