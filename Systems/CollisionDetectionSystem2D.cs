using System.Numerics;
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
        var mesh1Pos = mesh1.Owner.Components.OfType<Position2>().FirstOrDefault();
        var mesh2Pos = mesh2.Owner.Components.OfType<Position2>().FirstOrDefault();

        if (mesh1Pos == null || mesh2Pos == null)
            return false;

        var mesh1Circle = mesh1.GetBoundingCircle();
        var mesh2Circle = mesh2.GetBoundingCircle();

        if (!Raylib.CheckCollisionCircles(mesh1Circle.Center + mesh1Pos.Position,
                mesh1Circle.Radius,
                mesh2Circle.Center + mesh2Pos.Position,
                mesh2Circle.Radius)
            )
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

    private bool DetectVertexCollision(CircleVertex circle1, Position2 pos1, CircleVertex circle2, Position2 pos2)
    {
        return Raylib.CheckCollisionCircles(circle1.Center + circle1.Offset + pos1.Position,
            circle1.Radius,
            circle2.Center + circle1.Offset + pos2.Position,
            circle2.Radius);
    }

    private bool DetectVertexCollision(TriangleVertex triangle, Position2 pos1, CircleVertex circle, Position2 pos2)
    {
        var circleCenter = circle.Center + circle.Offset + pos2.Position;
        foreach (var point in triangle.Points)
        {
            if (Raylib.CheckCollisionPointCircle(point + triangle.Offset + pos1.Position, circleCenter, circle.Radius))
            {
                return true;
            }
        }
        return false;
    }

    private bool DetectVertexCollision(RectangleVertex rectangleVertex, Position2 pos1, CircleVertex circle, Position2 pos2)
    {
        return Raylib.CheckCollisionCircleRec(circle.Center+circle.Offset+pos2.Position,circle.Radius,rectangleVertex.Vertex);
    }

    private bool DetectVertexCollision(TriangleVertex triangle1, Position2 pos1, TriangleVertex triangle2, Position2 pos2)
    {
        Vector2[] triangle2Transformed = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            triangle2Transformed[i] = triangle2.Points[i] + triangle2.Offset + pos2.Position;
        }
        foreach (var point in triangle1.Points)
        {
            if (Raylib.CheckCollisionPointTriangle(point + triangle1.Offset + pos1.Position,
                    triangle2Transformed[0],
                    triangle2Transformed[1],
                    triangle2Transformed[2]))
            {
                return true;
            }
        }

        return false;
    }

    private bool DetectVertexCollision(RectangleVertex rect1, Position2 pos1, RectangleVertex rect2, Position2 pos2)
    {
        
    }
    private bool DetectVertexCollision(TriangleVertex triangle, Position2 pos1, RectangleVertex rect, Position2 pos2)
    {

    }
}

public struct Circle
{
    public Vector2 Center;
    public float Radius;

    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }
}