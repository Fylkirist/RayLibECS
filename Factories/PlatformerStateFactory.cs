using RayLibECS.EntityStates;
using RayLibECS.Interfaces;

namespace RayLibECS.Factories;

internal class PlatformerStateFactory : IStateFactory
{
    private Dictionary<string, EntityStateBase> _cachedStates;

    internal PlatformerStateFactory()
    {
        _cachedStates = new Dictionary<string, EntityStateBase>();
    }
    public EntityStateBase CreateState(string state)
    {
        switch(state){
            case "idle":
                return _cachedStates.ContainsKey("idle")
                    ? _cachedStates["idle"]
                    : NewState("idle", new IdleStateBase());
            case "running":
                return _cachedStates.ContainsKey("running")
                    ? _cachedStates["running"]
                    : NewState("running", new RunningStateBase());
            case "jumping":
                return _cachedStates.ContainsKey("jumping")
                    ? _cachedStates["jumping"]
                    : NewState("jumping", new JumpingStateBase());

        }
        throw new Exception("invalid state");
    }

    public EntityStateBase NewState(string key,EntityStateBase stateBase)
    {
        _cachedStates.Add(key,stateBase);
        return _cachedStates[key];
    }
}
