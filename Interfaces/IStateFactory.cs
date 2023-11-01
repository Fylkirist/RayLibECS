namespace RayLibECS.Interfaces;

public interface IStateFactory
{
    public ICharacterState CreateState(string state);
}
