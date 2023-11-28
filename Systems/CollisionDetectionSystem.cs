using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Events;

namespace RayLibECS.Systems;

public class CollisionDetectionSystem : SystemBase{
    private bool _running;
    private double _simulationDistance;
    
    public CollisionDetectionSystem(World world, PhysicsMode mode) : base(world)
    {
        _running = false;       
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Draw()
    {
        
    }

    public override void Initialize()
    {
       _running = true; 
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
        foreach(var leftover in World.GetComponents<CollisionEvent>()){
            World.DetachComponent(leftover);
        }
        if(!_running) return;
        DetectCollisions2D();
        DetectCollisions3D();
    }

    private void DetectCollisions2D(){
        var physicalEntities = World.GetEntitiesWith<Physics2>().ToArray();
        foreach(Entity entity1 in physicalEntities){
            foreach(Entity entity2 in physicalEntities){
                if(entity1 == entity2) continue;

            }
        }
    }

    private void DetectCollisions3D(){
        var physicalEntities = World.GetComponents<Physics3>().ToArray();
        foreach(var collider1 in physicalEntities){
            foreach(var collider2 in physicalEntities){

            }
        }
    }
}
