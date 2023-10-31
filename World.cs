using System.Numerics;
using System.Runtime.InteropServices;
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
    private Dictionary<Type, object> _componentTable;
    public Dictionary<Type, object> ComponentTable => _componentTable;

    private List<Entity> _entities;
    public List<Entity> Entities => _entities;

    private List<SystemBase> _systems;

    private List<dynamic> _componentCache;

    private InputState _inputState;
    public InputState InputState => _inputState;

    private int _entityLimit;

    private ComponentManager _componentManager;
    public World()
    {
        _componentManager = new ComponentManager();
        _entityLimit = 1000;
        _componentTable = new Dictionary<Type, object>();
        _entities = new List<Entity>();
        _systems = new List<SystemBase>();
        _componentCache = new List<dynamic>();
        _inputState = new InputState();
    }

    public void InitializeWorld()
    {
        var testEntity1 = CreateEntity("");

        var testRender1 = CreateComponent<ColouredMesh2>();
        
        testRender1.Colours.Add(Color.WHITE);
        testRender1.Colours.Add(Color.BLUE);

        testRender1.Mesh.Shapes.Add(new CircleGeometry(200,new Vector2(0,0)));
        testRender1.Mesh.Shapes.Add(new CircleGeometry(200, new Vector2(180, 50)));
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
        
        testPhysics1.Velocity = new Vector2(200,-300);
        testPhysics1.PhysicsType = PhysicsType2D.Dynamic;
        testPhysics1.Rotation = 0f;
        testPhysics1.CollisionMesh.Shapes.Add(new CircleGeometry(200,Vector2.Zero));
        testPhysics1.CollisionMesh.Shapes.Add(new CircleGeometry(200,new Vector2(180,50)));
        testPhysics1.RotationSpeed = 1f;
        testPhysics1.Position = new Vector2(-400,0);
        testPhysics1.Mass = 20f;

        AttachComponents(
            testEntity1,
            CreateComponent<Moveable>(),
            testPhysics1,
            testRender1
            );
        
        var testEntity2 = CreateEntity("");

        var testRender2 = CreateComponent<ColouredMesh2>();
        testRender2.Mesh.Shapes.Add(new CircleGeometry(200,new Vector2(0,0))); 

        testRender2.Colours.Add(Color.RED);
        testRender2.Colours.Add(Color.BLUE);

        var testPhysics2 = CreateComponent<Physics2>();

        testPhysics2.Velocity = new Vector2(-200,-300);
        testPhysics2.Position = new Vector2(300,100);
        testPhysics2.PhysicsType = PhysicsType2D.Dynamic;
        testPhysics2.Rotation = 0f;
        testPhysics2.CollisionMesh.Shapes.Add(new CircleGeometry(200,Vector2.Zero));
        testPhysics2.Mass = 10f;

        AttachComponents(
                testEntity2,
                testPhysics2,
                testRender2
                );
        
        var testEntity3 = CreateEntity("");

        var testRender3 = CreateComponent<ColouredMesh2>();
        testRender3.Colours.Add(Color.SKYBLUE);
        testRender3.Mesh.Shapes.Add(new CircleGeometry(300, Vector2.Zero));

        var testPhysics3 = CreateComponent<Physics2>();
        testPhysics3.Mass = 30f;
        testPhysics3.Velocity = new Vector2(0,-600);
        testPhysics3.Position = new Vector2(0,800);
        testPhysics3.PhysicsType = PhysicsType2D.Dynamic;
        testPhysics3.CollisionMesh.Shapes.Add(new CircleGeometry(300, Vector2.Zero));
        
        AttachComponents(testEntity3,
                testRender3,
                testPhysics3);
        
        var testEntity4 = CreateEntity("");

        var testRender4 = CreateComponent<ColouredMesh2>();
        testRender4.Colours.Add(Color.SKYBLUE);
        testRender4.Mesh.Shapes.Add(new CircleGeometry(50, Vector2.Zero));

        var testPhysics4 = CreateComponent<Physics2>();
        testPhysics4.Mass = 5f;
        testPhysics4.Velocity = new Vector2(0,300);
        testPhysics4.Position = new Vector2(300,-500);
        testPhysics4.PhysicsType = PhysicsType2D.Dynamic;
        testPhysics4.CollisionMesh.Shapes.Add(new CircleGeometry(50, Vector2.Zero));
        
        AttachComponents(testEntity4,
                testRender4,
                testPhysics4);


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
                DetachComponent(array.Key,entity.Id);
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

    public T CreateComponent<T>() where T : new()
    {
        var newComponent = new T();
        return newComponent;
    }

    public void AttachComponents(Entity entity,params dynamic[] components)
    {
        foreach (var component in components)
        {
            component.Owner = entity.Id;
            if (!_componentTable.ContainsKey(component.GetType()))
            {
                _componentTable.Add(component.GetType(), Array.CreateInstance(component.GetType(),_entityLimit));
                _componentTable[component.GetType()].Initialize();
            }
            _componentTable[component.GetType()][entity.Id] = component;
            var type = component.GetType();
            _componentManager.ActivateComponent(type,entity.Id);
        }
    }

    public void DetachComponent<T>(int id)
    {
        _componentManager.DeactivateComponent<T>(id);
    }

    public void DetachComponent(Type type, int id)
    {
        _componentManager.DeactivateComponent(type,id);
    }

    public void ClearComponents<T>()
    {
        _componentManager.DetachAll<T>();
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

    public T[] GetComponents<T>()
    {
        if (_componentTable.TryGetValue(typeof(T), out var componentArray))
        {
            return (T[])_componentTable[typeof(T)];
        }

        return Array.Empty<T>();
    }


    public bool IsComponentActive<T>(int id)
    {
        return _componentManager.IsComponentActive<T>(id);
    }
    public bool IsComponentActive(Type type,int id)
    {
        return _componentManager.IsComponentActive(type,id);
    }

    public IEnumerable<dynamic> GetComponents(int id)
    {
        return Array.Empty<dynamic>();
    }

    public IEnumerable<Entity> GetEntities(string tag)
    {
        return _entities.Where(e => e.Tag == tag);
    }

    public IEnumerable<Entity> GetEntitiesWith<T>()
    {
        return _entities.Where(e => IsComponentActive<T>(e.Id));
    }
}
