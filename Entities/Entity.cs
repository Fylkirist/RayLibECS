namespace RayLibECS.Entities;

public struct Entity
{
    public int Id;
    public string Tag;
    public bool ToBeDestroyed;
    private static Entity? _placeholder;
    public static Entity Placeholder => _placeholder ??= new Entity(-1,"Placeholder");

    public Entity(int id, string tag)
    {
        Id = id;
        Tag = tag;
        ToBeDestroyed = false;
    }
}