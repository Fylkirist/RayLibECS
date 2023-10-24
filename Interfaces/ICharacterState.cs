using RayLibECS.Entities;
using RayLibECS;

namespace Interfaces;

public abstract class CharacterState
{
    public string[] ValidSuccessors;
    
    public string State;

    protected CharacterState(string[] successors, string state)
    {
        ValidSuccessors = successors;
        State = state;
    }

    public abstract void EnterState(World world, Entity entity);

    public abstract void Update(World world, Entity entity);

    public abstract void ExitState(World world, Entity entity);
}
