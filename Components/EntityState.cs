namespace RayLibECS.Components;

public class EntityState : Component
{
    public string CurrentState;
    public string LastUpdate;

    public string EntityCategory;

    public EntityState(){
        CurrentState = "";
        LastUpdate = "";
        EntityCategory = "";
    }
}