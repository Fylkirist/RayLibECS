using RayLibECS.Components;
using RayLibECS.Interfaces;

namespace RayLibECS.Systems;


public class EntityStateManagementSystem:SystemBase
{

    private Dictionary<string,IEntityState> _stateDict;
    private bool _active;

    public EntityStateManagementSystem(World world, Dictionary<string,IEntityState> states):base(world)
    {
        _stateDict = states;
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

        foreach(var entityState in World.GetComponents<EntityStateType>())
        {
            
            if(!_stateDict.TryGetValue(entityState.EntityCategory, out var state)) break;
            state.Update(World,entityState.Owner,World.InputState,delta);
        }
    }

    public override void Detach()
    {
            
    }

    public override void Initialize()
    {
        _active = true;
    }
}
