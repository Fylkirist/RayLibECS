namespace RayLibECS.Interfaces;

public interface IStateFactory
{
    public EntityStateBase CreateState(string state);
}
