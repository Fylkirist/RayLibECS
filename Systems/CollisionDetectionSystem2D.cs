using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Vertices;

namespace RayLibECS.Systems;
public class CollisionDetectionSystem2D : System
{
    private bool _active;
    public CollisionDetectionSystem2D(World world) : base(world)
    {
        _active = false;
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
        _active = false;
    }

    public override void Initialize()
    {
        _active = true;
    }

    private bool DetectVertexCollision(CircleVertex circle1, Position2 pos1, CircleVertex circle2, Position2 pos2)
    {
        
        return Raylib.CheckCollisionCircles(
            ApplyRotationMatrix(
                circle1.Center + circle1.Offset + pos1.Position,
                pos1.Position,pos1.Rotation),
            circle1.Radius,
            ApplyRotationMatrix(
                circle2.Center + circle1.Offset + pos2.Position,
                pos2.Position,pos2.Rotation),
            circle2.Radius);
    }

    private bool DetectVertexCollision(TriangleVertex triangle, Position2 pos1, CircleVertex circle, Position2 pos2)
    {

        var circleCenter = ApplyRotationMatrix(
            circle.Center + circle.Offset + pos2.Position,
            pos2.Position,
            pos2.Rotation
            );

        
    }

    private bool DetectVertexCollision(RectangleVertex rectangleVertex, Position2 pos1, CircleVertex circle, Position2 pos2)
    {
        var rectangleCenter = rectangleVertex.Offset + pos1.Position;
        var rectangle = new Rectangle(
            rectangleVertex.Vertex.x+rectangleVertex.Offset.X+pos1.Position.X,
            rectangleVertex.Vertex.y+rectangleVertex.Offset.Y+pos1.Position.Y,
            rectangleVertex.Vertex.width,
            rectangleVertex.Vertex.height
            );
        var circleCenter =
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    circle.Center + circle.Offset + pos2.Position,
                    pos2.Position,
                    pos2.Rotation),
                rectangleCenter,
                rectangleVertex.Rotation
            );
        return Raylib.CheckCollisionCircleRec(circleCenter, circle.Radius,rectangle);
    }

    private bool DetectVertexCollision(TriangleVertex triangle1, Position2 pos1, TriangleVertex triangle2, Position2 pos2)
    {
        Vector2[] triangle1Transformed = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            triangle1Transformed[i] = ApplyRotationMatrix(
                ApplyRotationMatrix(
                    triangle1.Points[i] + triangle1.Offset + pos1.Position,
                    triangle1.Offset+pos1.Position,
                    triangle1.Rotation),
                pos1.Position,
                pos1.Rotation
                );
        }

        Vector2[] triangle2Transformed = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            triangle2Transformed[i] = ApplyRotationMatrix(
                ApplyRotationMatrix(
                    triangle2.Points[i] + triangle2.Offset + pos2.Position,
                    triangle2.Offset + pos2.Position,
                    triangle2.Rotation),
                pos2.Position,
                pos2.Rotation
            );
        }

        foreach (var point in triangle1Transformed)
        {
            if (Raylib.CheckCollisionPointTriangle(point, triangle2Transformed[0], triangle2Transformed[1],
                    triangle2Transformed[2]))
            {
                return true;
            }
        }

        foreach (var point in triangle2Transformed)
        {
            if (Raylib.CheckCollisionPointTriangle(point, triangle1Transformed[0], triangle1Transformed[1],
                    triangle1Transformed[2]))
            {
                return true;
            }
        }
        return false;
    }

    private bool DetectVertexCollision(RectangleVertex rect1, Position2 pos1, RectangleVertex rect2, Position2 pos2)
    {
        return true;
    }
    private bool DetectVertexCollision(TriangleVertex triangle, Position2 pos1, RectangleVertex rect, Position2 pos2)
    {
        return true;
    }

    public static Vector2 ApplyRotationMatrix(Vector2 point,Vector2 center,float rotation)
    {
        var transformation = Matrix3x2.CreateRotation(rotation, center);
        return Vector2.Transform(point, transformation);
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