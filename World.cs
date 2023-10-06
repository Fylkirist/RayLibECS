using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Systems;
using RayLibECS.Vertices;

namespace RayLibECS;

public enum RenderingModes
{
    TwoD,
    ThreeD,
    Static
}
public class World
{
    private List<Entity> _entities;
    public List<Entity> Entities => _entities;
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

    public void InitializeWorld()
    {
        var testEntity = CreateEntity("stuff");

        var testMass = CreateComponent<Mass>();
        testMass.Kgs = 100;

        var testRenderComponent = CreateComponent<DrawableCircle>();
        testRenderComponent.Position = Vector2.Zero;
        testRenderComponent.Radius = 200;
        testRenderComponent.Colour = Color.WHITE;

        var testCollisionMesh1 = CreateComponent<CollisionMesh2>();
        testCollisionMesh1.Vertices.Add(new CircleVertex(Vector2.Zero, 200,Vector2.Zero));

        var testPosition1 = CreateComponent<Position2>();
        testPosition1.Position = new Vector2(150,150);

        var testEntity2 = CreateEntity("stuff");

        var testRenderComponent2 = CreateComponent<DrawableCircle>();
        testRenderComponent2.Position = Vector2.Zero;
        testRenderComponent2.Radius = 200;
        testRenderComponent2.Colour = Color.WHITE;

        var testCollisionMesh2 = CreateComponent<CollisionMesh2>();
        testCollisionMesh2.Vertices.Add(new CircleVertex(Vector2.Zero, 200, Vector2.Zero));

        var testPosition2 = CreateComponent<Position2>();
        testPosition2.Position = Vector2.Zero;

        var testEntity3 = CreateEntity("stuff");

        var testRenderComponent3 = CreateComponent<DrawableCircle>();
        testRenderComponent3.Position = Vector2.Zero;
        testRenderComponent3.Radius = 200;
        testRenderComponent3.Colour = Color.RED;

        var testCollisionMesh3 = CreateComponent<CollisionMesh2>();
        testCollisionMesh3.Vertices.Add(new CircleVertex(Vector2.Zero, 200, Vector2.Zero));

        var testPosition3 = CreateComponent<Position2>();
        testPosition3.Position = new Vector2(300,300);

        AttachComponent(testEntity,testRenderComponent);
        AttachComponent(testEntity,testCollisionMesh1);
        AttachComponent(testEntity,testPosition1);
        AttachComponent(testEntity,testMass);

        AttachComponent(testEntity2,testCollisionMesh2);
        AttachComponent(testEntity2, testRenderComponent2);
        AttachComponent(testEntity2,testPosition2);

        AttachComponent(testEntity3, testCollisionMesh3);
        AttachComponent(testEntity3, testRenderComponent3);
        AttachComponent(testEntity3, testPosition3);

        AddSystem(new RenderingSystem2D(this));
        AddSystem(new CollisionDetectionSystem2D(this));
        AddSystem(new PhysicsSystem2D(this));


        AddSystem(new MovementSystem(this));
    }

    public void Update(float delta,InputState input)
    {
        foreach (var system in _systems)
        {
            system.Update(delta,input);
        }

        foreach (var entity in _entitiesToDestroy)
        {
            foreach (var component in entity.Components)
            {
                DetachComponent(component);
            }
            _entities.Remove(entity);
            _entitiesToDestroy.Remove(entity);
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
        if (entity.Components.Any(c=>c.GetType() == component.GetType()))
        {
            return;
        }
        component.Owner = entity;
        _components.Add(component);
        entity.Components.Add(component);
    }

    public void DetachComponent(Component component)
    {
        _components.Remove(component);
        _cachedComponents.Add(component);
        if (_cachedComponents.Count > 1000)
        {
            _cachedComponents.RemoveAt(0);
        }
    }

    public void AddSystem(Systems.System system)
    {
        _systems.Add(system);
        system.Initialize();
    }

    public void RemoveSystem(Systems.System system)
    {
        
    }

    public IEnumerable<T> GetComponents<T>()
    {
        return _components.OfType<T>();
    }

    public IEnumerable<Component> GetComponents(int id)
    {
        return _components.Where(c => c.Owner.Id == id);
    }

    public IEnumerable<Component> GetComponents(string tag)
    {
        return _components.Where(c => c.Owner.Tag == tag);
    }

    public IEnumerable<Component> GetComponents(Entity entity)
    {
        return entity.Components;
    }

    public IEnumerable<Component> GetComponents(RenderingModes renderMode)
    {
        return _components
            .OfType<RenderableComponent>()
            .Where(c => c.RenderingMode == renderMode);
    }

    public IEnumerable<Component> GetCollisionShapes(Type[] collisionTypes)
    {
        return _components.Where(c=>collisionTypes.Contains(c.GetType()));
    }

    public IEnumerable<Entity> GetEntitiesWith<T>()
    {
        return _entities.Where(e => e.Components.OfType<T>().Any());
    }
}