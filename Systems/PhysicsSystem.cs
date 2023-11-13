using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;

public enum PhysicsMode{
    TWO_DIMENSIONAL,
    THREE_DIMENSIONAL
}

public class PhysicsSystem : SystemBase{
    private bool _running;
    private PhysicsMode _mode;
    
    public PhysicsSystem(World world, PhysicsMode mode) : base(world){
        _running = false;
        _mode = mode;
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Draw()
    {
        
    } 

    public override void Initialize(){
        _running = true;
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
        throw new NotImplementedException();
    }

    public void HandleCollisions(){
        var collisions = World.GetComponents<CollisionEvent>();

    }

    public void HandleMovement2D(){
       var physicsComponents = World.GetComponents<Physics2>();
       foreach(var component in physicsComponents){
           component.Position += component.Velocity;
           component.Rotation += component.RotationSpeed;
       }
    }

    public void HandleMovement3D(){
        var physicsComponents = World.GetComponents<Physics3>();

    }
}
