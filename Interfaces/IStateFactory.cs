namespace RayLibECS.Interfaces;

public interface IStateFactory
{
    public IEntityState CreateState(string state);
}
