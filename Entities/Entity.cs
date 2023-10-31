namespace RayLibECS.Entities;

public struct Entity
{
    public int Id;
    public string Tag;
    public bool ToBeDestroyed;

    public Entity(int id, string tag)
    {
        Id = id;
        Tag = tag;
        ToBeDestroyed = false;
    }
}