using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;
using RayLibECS.Systems;

namespace RayLibECS;

public enum RenderingModes
{
    TwoD,
    ThreeD,
    Static
}
public class World
{
    private Dictionary<Type, Component?[]> _componentTable;
    public Dictionary<Type, Component?[]> ComponentTable => _componentTable;

    private List<Entity> _entities;
    public List<Entity> Entities => _entities;

    private List<Systems.SystemBase> _systems;

    private List<Component> _cachedComponents;

    private InputState _inputState;
    public InputState InputState => _inputState;

    private int _entityLimit;
    public World()
    {
        _componentTable = new Dictionary<Type, Component?[]>();
        _entities = new List<Entity>();
        _systems = new List<Systems.SystemBase>();
        _cachedComponents = new List<Component>();
        _inputState = new InputState();
        _entityLimit = 1000;
    }

    public void InitializeWorld()
    {
        AddSystem(new RenderingSystem2D(this));
        AddSystem(new CollisionDetectionSystem2D(this));
        AddSystem(new PhysicsSystem2D(this));


        AddSystem(new MovementSystem(this));
    }

    public void Update(float delta)
    {
        _inputState.Update();
        foreach (var system in _systems)
        {
            system.Update(delta);
        }

        foreach (var entity in _entities.Where(entity => entity.ToBeDestroyed))
        {
            foreach (var array in _componentTable)
            {
                var component = array.Value[entity.Id];
                if (component != null)
                { 
                    DetachComponent(component);
                }
            }
            _entities.Remove(entity);
        }
    }

    public void Draw()
    {
        Raylib.ClearBackground(Color.BLACK);
        foreach (var system in _systems)
        {
            system.Draw();
        }
    }

    public void DestroyEntity(Entity entity)
    {
        entity.ToBeDestroyed = true;
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

    public T CreateComponent<T>() where T : Component, new()
    {
        var cached = _cachedComponents.OfType<T>().FirstOrDefault();
        if (cached != null)
        {
            _cachedComponents.Remove(cached);
            return cached;
        }

        if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            var newComponent = new T();
            return newComponent;
        }

        throw new InvalidOperationException($"Type {typeof(T)} is not a valid Component type.");
    }

    public void AttachComponent(Entity entity,Component component)
    {
        component.Owner = entity;
        if (!_componentTable.ContainsKey(component.GetType()))
        {
            _componentTable.Add(component.GetType(),new Component[_entityLimit]);
        }
        _componentTable[component.GetType()][entity.Id] = component;
    }

    public void DetachComponent(Component component)
    {
        _componentTable[component.GetType()][component.Owner.Id] = null;
        _cachedComponents.Add(component);
        if (_cachedComponents.Count > 1000)
        {
            _cachedComponents.RemoveAt(0);
        }
    }

    public void AddSystem(Systems.SystemBase systemBase)
    {
        _systems.Add(systemBase);
        systemBase.Initialize();
    }

    public void RemoveSystem(Systems.SystemBase systemBase)
    {
        
    }

    public void StopSystem<T>()
    {
        
    }

    public T? QueryComponent<T>(Entity entity) where T : Component
    {
        if( !_componentTable.ContainsKey(typeof(T))) return null;
        var component = _componentTable[typeof(T)][entity.Id];
        if (component == null)
        {
            return null;
        }
        return (T)component;
    }

    public IEnumerable<T> GetComponents<T>()
    {
        return _componentTable[typeof(T)].Cast<T>().Where(e=>e!=null);
    }

    public IEnumerable<Component> GetComponents(int id)
    {
        var components = new List<Component>();
        foreach (var array in _componentTable)
        {
            var component = array.Value[id];
            if (component != null)
            {
                components.Add(component);
            }
        }
        return components;
    }

    public IEnumerable<Component> GetComponents(string tag)
    {
        var components = new List<Component>();
        foreach (var entity in Entities.Where(e => e.Tag == tag))
        {
            components.AddRange(GetComponents(entity));
        }
        return components;
    }

    public IEnumerable<Component> GetComponents(Entity entity)
    {
        var components = new List<Component>();
        foreach (var array in _componentTable)
        {
            var component = array.Value[entity.Id];
            if (component != null)
            {
                components.Add(component);
            }
        }
        return components;
    }

    public IEnumerable<Entity> GetEntitiesWith<T>()
    {
        return _entities.Where(e => _componentTable[typeof(T)][e.Id] != null);
    }
}