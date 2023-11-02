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
        throw new NotImplementedException();
    }
}