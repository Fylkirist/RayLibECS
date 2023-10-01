using RayLibECS.Components;

namespace RayLibECS.Systems;
public class CollisionDetectionSystem2D : System
{
    private Type[] _collisionTypes;

    public CollisionDetectionSystem2D(World world) : base(world)
    {
        _collisionTypes = new []
        {
            typeof(Polygon2CollisionShape),
            typeof(RectangleCollisionShape),
            typeof(BoxCollisionShape)
        };
    }
    public override void Draw()
    {
        
    }

    public override void Update(float delta,InputState input)
    {
        var collisionShapes = World.GetCollisionShapes(_collisionTypes);
        foreach (var entity1 in collisionShapes)
        {
            foreach (var entity2 in collisionShapes)
            {
                if(entity1 == entity2) continue;
                foreach (var type in _collisionTypes)
                {
                    
                }
            }
        }
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

}