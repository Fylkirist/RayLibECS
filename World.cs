﻿using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;
using RayLibECS.Systems;
using RayLibECS.Interfaces;
using RayLibECS.Events;

namespace RayLibECS;

public class World
{
    private static World? _world;
    public static World? Instance => _world;

    private Dictionary<Type, Component?[]> _componentTable;
    public Dictionary<Type, Component?[]> ComponentTable => _componentTable;

    private Dictionary<Type, List<WorldEvent>> _eventTable;

    private List<Entity> _entities;
    public List<Entity> Entities => _entities;

    private List<SystemBase> _systems;

    private List<Component> _componentCache;

    private InputState _inputState;
    public InputState InputState => _inputState;

    private int _entityLimit;

    public AssetManager AssetManager;
    public World()
    {
        _eventTable = new Dictionary<Type, List<WorldEvent>>();
        AssetManager = new AssetManager(2147483648,2147483648);
        _entityLimit = 1000;
        _componentTable = new Dictionary<Type, Component?[]>();
        _entities = new List<Entity>();
        _systems = new List<SystemBase>();
        _componentCache = new List<Component>();
        _inputState = new InputState();
        _world = this;
    }

    public void InitializeWorld()
    {

        var entity1 = CreateEntity("test");

        var physics1 = CreateComponent<Physics2>();
        physics1.Position = new Vector2(0, 400);
        physics1.Z = 0;
        physics1.SetPhysicsFlags(new KeyValuePair<PhysicsFlags,bool>[]{
                new (PhysicsFlags.Collidable, true),
                new (PhysicsFlags.Static, true),
                new (PhysicsFlags.Gravity, false)
                });

        var rigidbody1 = CreateComponent<RigidBody2>();

        rigidbody1.AngularVelocity = 0;
        rigidbody1.Shapes = new Shape2D[1];
        rigidbody1.Shapes[0] = new Shape2D
        {
            Type = ShapeType2D.Rectangle,
            Offset = Vector2.Zero,
            Rectangle = new BasedRectangle(100, 1000)
        };


        var entity2 = CreateEntity("test");

        var physics2 = CreateComponent<Physics2>();
        physics2.Position = Vector2.Zero;
        physics2.Z = 0;
        physics2.SetPhysicsFlags(new KeyValuePair<PhysicsFlags,bool>[]{
                new (PhysicsFlags.Collidable, true),
                new (PhysicsFlags.Movable, true),
                new (PhysicsFlags.Gravity, true)
                });

        var rigidbody2 = CreateComponent<RigidBody2>();

        rigidbody2.AngularVelocity = 0f;
        rigidbody2.Shapes = new Shape2D[1];
        rigidbody2.Shapes[0] = new Shape2D
        {
            Type = ShapeType2D.Rectangle,
            Offset = Vector2.Zero,
            Rectangle = new BasedRectangle(200,200)
        };

        var testState2 = CreateComponent<EntityStateType>();
        testState2.EntityCategory = "platformer";

        var testStats2 = CreateComponent<CharacterStats>();
        testStats2.JumpHeight = 400;
        testStats2.Speed = 200;

        AttachComponents(entity2,physics2,rigidbody2,testState2);
        AttachComponents(entity1,physics1,rigidbody1);

        AddSystem(new CollisionDetectionSystem(this));
        AddSystem(new PhysicsSystem(this,10000,100));
        AddSystem(new EntityStateManagementSystem(this, new Dictionary<string, IEntityState>()));
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

    public void ClearEvents<T>()
    {
        if (!_eventTable.ContainsKey(typeof(T)))
        {
            return;
        }
        _eventTable[typeof(T)].Clear();
    }

    public void PublishEvent<T>(T worldEvent) where T : WorldEvent
    {
        if (!_eventTable.ContainsKey(typeof(T)))
        {
            _eventTable.Add(typeof(T),new List<WorldEvent>());
        }
        _eventTable[typeof(T)].Add(worldEvent);
    }

    public IEnumerable<T> GetWorldEvents<T>() where T : WorldEvent
    {
        return !_eventTable.ContainsKey(typeof(T)) ? Array.Empty<T>() : _eventTable[typeof(T)].Cast<T>();
    }

    public void AllocateComponentArray<T>()
    {
        if (_componentTable.ContainsKey(typeof(T))) return;
        _componentTable.Add(typeof(T), new Component[_entityLimit]);
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
        component.Owner = Entity.Placeholder;
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

    public List<RenderableComponent2> GetRenderables2()
    {
        List<RenderableComponent2> renderables = new List<RenderableComponent2>();
        Type[] renderableTypes = {typeof(MappedTexture2),typeof(TexturedMesh2),typeof(AnimatedSprite2)};
        
        foreach(var type in renderableTypes){
            renderables.AddRange(
                (IEnumerable<RenderableComponent2>)(_componentTable.ContainsKey(type) ?
                    _componentTable[type].Where(e => e != null) :
                    Array.Empty<RenderableComponent2>()));
        }

        return renderables;
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
        if (!_componentTable.ContainsKey(typeof(T)))
        {
            AllocateComponentArray<T>();
        }
        return _entities.Where(e => _componentTable[typeof(T)][e.Id] != null);
    }
}
