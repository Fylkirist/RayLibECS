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
    private double _simulationDistance;
    
    public PhysicsSystem(World world, double simDistance) : base(world){
        _running = false;
        _simulationDistance = simDistance;
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
        HandleMovement3D(delta);
        HandleMovement2D(delta);
        HandleCollisions();
    }

    public void HandleCollisions(){
        var collisions = World.GetComponents<CollisionEvent>();
        foreach(var collision in collisions){
            
        }
    }

    public void HandleMovement2D(float delta){
       var physicsComponents = World.GetComponents<Physics2>();
       foreach(var component in physicsComponents){
           component.Position += component.Velocity * delta;
           component.Rotation += component.RotationSpeed * delta;
       }
    }

    public void HandleMovement3D(float delta){
        var physicsComponents = World.GetComponents<Physics3>();
        foreach (var component in physicsComponents)
        {
           component.Position += component.Velocity * delta;
           component.Rotation += component.AngularMomentum * delta;

        }
    }
}
