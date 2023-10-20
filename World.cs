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

    private List<SystemBase> _systems;

    private List<Component> _componentCache;

    private InputState _inputState;
    public InputState InputState => _inputState;

    private int _entityLimit;
    public World()
    {
        _componentTable = new Dictionary<Type, Component?[]>();
        _entities = new List<Entity>();
        _systems = new List<SystemBase>();
        _componentCache = new List<Component>();
        _inputState = new InputState();
        _entityLimit = 1000;
    }

    public void InitializeWorld()
    {
        var testEntity1 = CreateEntity("");

        var testRender1 = CreateComponent<ColouredMesh2>();
        
        testRender1.Colours.Add(Color.WHITE);
        testRender1.Colours.Add(Color.BLUE);

        testRender1.Mesh.Shapes.Add(new CircleGeometry(200,new Vector2(0,0)));
        testRender1.Mesh.Shapes.Add(new TriangleGeometry(
                    new Vector2[]{
                        new(-200,-200),
                        new(-200,0),
                        new(0,0)
                    },
                    new Vector2(-400,0),
                    2f
                ));
        var testPhysics1 = CreateComponent<Physics2>();
        
        testPhysics1.Velocity = new Vector2(0,0);
        testPhysics1.PhysicsType = PhysicsType2D.Static;
        testPhysics1.Rotation = 0f;
        testPhysics1.CollisionMesh.Shapes.Add(new CircleGeometry(200,Vector2.Zero));
        testPhysics1.RotationSpeed = 1f;
        testPhysics1.Position = new Vector2(-400,0);
        testPhysics1.Mass = 10f;

        AttachComponents(
            testEntity1,
            CreateComponent<Moveable>(),
            testPhysics1,
            testRender1
            );
        
        var testEntity2 = CreateEntity("");
        
        var testRender2 = CreateComponent<ColouredMesh2>();
        testRender2.Mesh.Shapes.Add(new CircleGeometry(200,new Vector2(0,0)));
        
        
        testRender2.Colours.Add(Color.BLUE);
        
        var testPhysics2 = CreateComponent<Physics2>();
        
        testPhysics2.Velocity = new Vector2(0,0);
        testPhysics2.Position = new Vector2(300,100);
        testPhysics2.PhysicsType = PhysicsType2D.Static;
        testPhysics2.Rotation = 0f;
        testPhysics2.CollisionMesh.Shapes.Add(new CircleGeometry(200,Vector2.Zero));
        testPhysics2.Mass = 10f;
        
        AttachComponents(
                testEntity2,
                testPhysics2,
                testRender2
                );
        

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

        int newId = 0;
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
        var cached = _componentCache.OfType<T>().FirstOrDefault();
        if (cached != null)
        {
            _componentCache.Remove(cached);
            return cached;
        }

        if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            var newComponent = new T();
            return newComponent;
        }

        throw new InvalidOperationException($"Type {typeof(T)} is not a valid Component type.");
    }

    public void AttachComponents(Entity entity,params Component[] components)
    {
        foreach (var component in components)
        {
            component.Owner = entity;
            if (!_componentTable.ContainsKey(component.GetType()))
            {
                _componentTable.Add(component.GetType(),new Component[_entityLimit]);
            }
            _componentTable[component.GetType()][entity.Id] = component;
        }
    }

    public void DetachComponent(Component component)
    {
        _componentTable[component.GetType()][component.Owner.Id] = null;
        _componentCache.Add(component);
        if (_componentCache.Count > 1000)
        {
            _componentCache.RemoveAt(0);
        }
    }

    public void AddSystem(SystemBase system)
    {
        _systems.Add(system);
        system.Initialize();
    }

    public void RemoveSystem(SystemBase system)
    {
        
    }

    public void StopSystem<T>() where T: SystemBase
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
        return !_componentTable.ContainsKey(typeof(T)) ? Array.Empty<T>() : _componentTable[typeof(T)].Cast<T>().Where(e=>e!=null);
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
