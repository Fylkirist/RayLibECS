using RayLibECS.EntityStates;
using RayLibECS.Interfaces;

namespace RayLibECS.Factories;

internal class PlatformerStateFactory : IStateFactory
{
    private Dictionary<string, IEntityState> _cachedStates;

    internal PlatformerStateFactory()
    {
        _cachedStates = new Dictionary<string, IEntityState>();
    }
    public IEntityState CreateState(string state)
    {
        switch(state){
            case "idle":
                return _cachedStates.ContainsKey("idle")
                    ? _cachedStates["idle"]
                    : NewState("idle", new IdleState());
            case "running":
                return _cachedStates.ContainsKey("running")
                    ? _cachedStates["running"]
                    : NewState("running", new RunningState());
            case "jumping":
                return _cachedStates.ContainsKey("jumping")
                    ? _cachedStates["jumping"]
                    : NewState("jumping", new JumpingState());

        }
        throw new Exception("invalid state");
    }

    public IEntityState NewState(string key,IEntityState state)
    {
        _cachedStates.Add(key,state);
        return _cachedStates[key];
    }
}