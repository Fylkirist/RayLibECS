using RayLibECS.Components;

namespace RayLibECS.Entities;

public class Entity
{
    public int Id;
    public string Tag;
    public bool ToBeDestroyed;
    private static Entity? _placeholder;
    public static Entity Placeholder => _placeholder ??= new Entity(0,"");

    public Entity(int id, string tag)
    {
        Id = id;
        Tag = tag;
        ToBeDestroyed = false;
    }
}