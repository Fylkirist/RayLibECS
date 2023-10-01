using RayLibECS.Components;

namespace RayLibECS.Entities;

public class Entity
{
    public int Id;
    public string Tag;
    public List<Component> Components;

    public Entity(int id, string tag)
    {
        Id = id;
        Tag = tag;
        Components = new List<Component>();
    }
}