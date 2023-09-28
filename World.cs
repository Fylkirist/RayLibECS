using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS;

public class World
{
    private List<Entity> _entities;
    private List<Entity> _entitiesToDestroy;
    private List<Systems.System> _systems;
    private List<Component> _components;
    private List<Component> _cachedComponents;

    public World()
    {
        _entities = new List<Entity>();
        _entitiesToDestroy = new List<Entity>();
        _systems = new List<Systems.System>();
        _components = new List<Component>();
        _cachedComponents = new List<Component>();
    }

    public void Update(long delta,InputState input)
    {
        foreach (var system in _systems)
        {
            system.Update(delta);
        }

        foreach (var entity in _entitiesToDestroy)
        {
            _components.RemoveAll(c => c.Owner == entity);
            _entities.Remove(entity);
            _entitiesToDestroy.Remove(entity);
        }
    }

    public void Draw()
    {
        foreach (var system in _systems)
        {
            system.Draw();
        }
    }

    public void DestroyEntity(Entity entity)
    {
        _entitiesToDestroy.Add(entity);
    }

    public Entity CreateEntity(string tag)
    {
        var usedIds = new HashSet<int>();
        foreach (var e in _entities)
        {
            usedIds.Add(e.Id);
        }

        int newId = 1;
        while (usedIds.Contains(newId))
        {
            newId++;
        }
        var entity = new Entity(newId,tag);
        _entities.Add(entity);
        return entity;
    }

    public Component CreateComponent(Type type)
    {
        var cached = _cachedComponents.FirstOrDefault(c => c.GetType() == type);
        if (cached != null)
        {
            _cachedComponents.Remove(cached);
            return cached;
        }
    
        if (typeof(Component).IsAssignableFrom(type))
        {
            var newComponent = Activator.CreateInstance(type) as Component;
            return newComponent;
        }
    
        throw new InvalidOperationException($"Type {type} is not a valid Component type.");
    }


    public void AttachComponent(Entity entity,Component component)
    {
        if (_components.Any(c => c.Owner == entity && component.GetType() == c.GetType()))
        {
            return;
        }
        component.Owner = entity;
        _components.Add(component);
    }

    public void DetachComponent(Component component)
    {
        _components.Remove(component);
        _cachedComponents.Add(component);
    }

    public void AddSystem(Systems.System system)
    {
        _systems.Add(system);
        system.Initialize();
    }

    public void RemoveSystem(Systems.System system)
    {

    }

    public IEnumerable<Component> GetComponents(Type type)
    {
        return _components.Where(component => component.GetType() == type);
    }

    public IEnumerable<Component> GetComponents(int id)
    {
        return _components.Where(c => c.Owner.Id == id);
    }

    public IEnumerable<Component> GetComponents(string tag)
    {
        return _components.Where(c => c.Owner.Tag == tag);
    }
}