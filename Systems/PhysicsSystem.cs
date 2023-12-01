using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;
using System.Numerics;
using RayLibECS.Events;

namespace RayLibECS.Systems;

public enum PhysicsMode{
    TWO_DIMENSIONAL,
    THREE_DIMENSIONAL
}

public class PhysicsSystem : SystemBase{
    
    private EventBus _eventBus;
    private float _physicsScale;
    private bool _running;
    private double _simulationDistance;
    private Vector3 _gravityVector; //Temporary solution
    
    public PhysicsSystem(World world, double simDistance, float scale) : base(world){
        _running = false;
        _simulationDistance = simDistance;
        _gravityVector = new Vector3(0,100,0);
        _physicsScale = scale;
        _eventBus = EventBus.Instance;
        _eventBus.Subscribe<CollisionEvent2>(HandleCollision2);
        _eventBus.Subscribe<CollisionEvent3>(HandleCollision3);
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
        UpdateSoftBodies2(delta);
        UpdateSoftBodies3(delta);
    }

    public void HandleCollision2(CollisionEvent2 collisionEvent){
        
    }

    public void HandleCollision3(CollisionEvent3 collisionEvent){

    }

    private void UpdateSoftBodies2(float delta){
        var softBodies2 = World.GetComponents<SoftBody2>();
        foreach(SoftBody2 softBody in softBodies2){
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].ForceVector.X = _gravityVector.X * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Y = _gravityVector.Y * softBody.Points[i].Mass; 
            }
            for(int i = 0; i<softBody.Springs.Length; i++){
                var currentSpringLength = (softBody.Points[softBody.Springs[i].Connection.X].PositionVector -
                        softBody.Points[softBody.Springs[i].Connection.Y].PositionVector).Length();
                var lengthdiff = currentSpringLength - softBody.Springs[i].RestLength;
                var massPointNormal = Vector2.Normalize(softBody.Points[softBody.Springs[i].Connection.X].PositionVector - softBody.Points[softBody.Springs[i].Connection.Y].PositionVector);
                var velocityDifference = softBody.Points[softBody.Springs[i].Connection.X].VelocityVector - softBody.Points[softBody.Springs[i].Connection.Y].VelocityVector;
                var springForce = (Vector2.Dot(massPointNormal,velocityDifference) * softBody.Springs[i].Damping) + lengthdiff * softBody.Springs[i].Stiffness;

                var springForceVector = springForce * massPointNormal;
                
                softBody.Points[softBody.Springs[i].Connection.X].ForceVector += springForceVector;
                softBody.Points[softBody.Springs[i].Connection.Y].ForceVector -= springForceVector;
            }
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].VelocityVector.X += (softBody.Points[i].ForceVector.X * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Y += (softBody.Points[i].ForceVector.Y * delta)/softBody.Points[i].Mass;
                softBody.Points[i].PositionVector += softBody.Points[i].VelocityVector;
            }
        }        
    }

    private void UpdateSoftBodies3(float delta){
        var softBodies3 = World.GetComponents<SoftBody3>();
        foreach(SoftBody3 softBody in softBodies3){
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].ForceVector.X = _gravityVector.X * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Y = _gravityVector.Y * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Z = _gravityVector.Z * softBody.Points[i].Mass;
            }
            for(int i = 0; i<softBody.Springs.Length; i++){
                var currentSpringLength = (softBody.Points[softBody.Springs[i].Connection.X].PositionVector -
                        softBody.Points[softBody.Springs[i].Connection.Y].PositionVector).Length();
                var lengthdiff = currentSpringLength - softBody.Springs[i].RestLength;
                var massPointNormal = Vector3.Normalize(softBody.Points[softBody.Springs[i].Connection.X].PositionVector - softBody.Points[softBody.Springs[i].Connection.Y].PositionVector);
                var velocityDifference = softBody.Points[softBody.Springs[i].Connection.X].VelocityVector - softBody.Points[softBody.Springs[i].Connection.Y].VelocityVector;
                var springForce = (Vector3.Dot(massPointNormal,velocityDifference) * softBody.Springs[i].Damping) + lengthdiff * softBody.Springs[i].Stiffness;

                var springForceVector = springForce * massPointNormal;
                
                softBody.Points[softBody.Springs[i].Connection.X].ForceVector += springForceVector;
                softBody.Points[softBody.Springs[i].Connection.Y].ForceVector -= springForceVector;
            }
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].VelocityVector.X += (softBody.Points[i].ForceVector.X * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Y += (softBody.Points[i].ForceVector.Y * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Z += (softBody.Points[i].ForceVector.Z * delta)/softBody.Points[i].Mass;
                softBody.Points[i].PositionVector += softBody.Points[i].VelocityVector;
            }
        }
    }

    public void HandleMovement2D(float delta){
       var physicsComponents = World.GetComponents<Physics2>();
       foreach(var component in physicsComponents){
           component.Position += component.Velocity * delta;
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
