using RayLibECS.Components;
using RayLibECS.Interfaces;

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

        foreach(var entityState in World.GetComponents<EntityState>())
        {
            entityState.StateIdentifiers = entityState.NextState.ToArray();
            entityState.NextState.Clear();

            for (var i = 0; i < entityState.StateIdentifiers.Length; i++)
            {
                var state = _factoryDict[entityState.EntityCategory].CreateState(entityState.StateIdentifiers[i]);
                state.Update(World,entityState.Owner,World.InputState,delta);
            }
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
