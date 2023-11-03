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
                return _cachedStates.ContainsKey("idle")? _cachedStates["idle"]: new IdleState();

            
        }
        throw new Exception("invalid state");
    }
}