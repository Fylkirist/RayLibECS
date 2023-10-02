using System.Runtime.CompilerServices;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Vertices;

namespace RayLibECS.Systems;
public class CollisionDetectionSystem2D : System
{

    public CollisionDetectionSystem2D(World world) : base(world)
    {

    }
    public override void Draw()
    {
        
    }

    public override void Update(float delta,InputState input)
    {
        var collisionMeshes = World.GetComponents(typeof(CollisionMesh2));
        foreach (CollisionMesh2 collisionMesh in collisionMeshes.Cast<CollisionMesh2>())
        {
            foreach (var component in collisionMeshes)
            {
                var colliderMesh = (CollisionMesh2)component;
                if(collisionMesh ==  colliderMesh) continue;
                if(colliderMesh.Owner.Components.Any(c=>c.GetType() == typeof(CollisionEvent))) continue;
                if (CheckMeshCollision(collisionMesh, colliderMesh))
                {
                    var collisionEvent = (CollisionMesh2)World.CreateComponent(typeof(CollisionMesh2));
                    World.AttachComponent(collisionMesh.Owner,collisionEvent);
                };
            }
        }
    }

    private bool CheckMeshCollision(CollisionMesh2 mesh1, CollisionMesh2 mesh2)
    {
        if(!Raylib.CheckCollisionRecs(mesh1.GetBoundingRect(), mesh2.GetBoundingRect()))
        {
            return false;
        }

        foreach (var vertex1 in mesh1.Vertices)
        {
            foreach (var vertex2 in mesh2.Vertices)
            {
                
            }
        }
        return true;
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