using RayLibECS.Components;
using RayLibECS.Interfaces;
using EntityState = RayLibECS.Components.EntityState;

namespace RayLibECS.Systems;


public class EntityStateManagementSystem:SystemBase
{

    private Dictionary<string,IStateFactory> _factoryDict;
    private bool _active;

    public EntityStateManagementSystem(World world, Dictionary<string,IStateFactory> stateFactories):base(world)
    {
        _factoryDict = stateFactories;
        _active = false;
    }

    public override void Draw()
    {

    }

    public override void Stop()
    {
        _active = false;
    }

    public override void Update(float delta)
    {
        if(!_active) return;

        foreach(var entityState in World.GetComponents<EntityState>()){
            
            var state = _factoryDict[entityState.EntityCategory].CreateState(entityState.CurrentState);
            
            if(entityState.CurrentState != entityState.LastUpdate)
            {
                var oldState = _factoryDict[entityState.EntityCategory].CreateState(entityState.LastUpdate);
                oldState.ExitState(World,entityState.Owner);
                
                state.EnterState(World,entityState.Owner);
            }
            state.Update(World,entityState.Owner, World.InputState, delta);
            
            entityState.LastUpdate = entityState.CurrentState;
        }
    }

    public override void Detach()
    {
            
    }

    public override void Initialize()
    {
        World.AllocateComponentArray<EntityState>();
        _active = true;
    }
}
