namespace RayLibECS.Components;

public class EntityState : Component
{
    public string[] StateIdentifiers;

    public List<string> NextState;

    public string EntityCategory;

    public EntityState(){
        StateIdentifiers = Array.Empty<string>();
        EntityCategory = "";
        NextState = new List<string>();
    }

    public void PushNextState(string stateId)
    {
        if (NextState.Contains(stateId))
        {
            return;
        }
        NextState.Add(stateId);
    }
}