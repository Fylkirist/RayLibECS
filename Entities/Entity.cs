namespace RayLibECS.Entities;

internal class Entity
{
    public int Id;
    public string Tag;

    public Entity(int id, string tag)
    {
        Id = id;
        Tag = tag;
    }
}