using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;
public class CollisionDetectionSystem2D : SystemBase
{
    private bool _active;
    public CollisionDetectionSystem2D(World world) : base(world)
    {
        _active = false;
    }

    public override void Draw()
    {
        
    }

    public override void Update(float delta)
    {
        if (!_active) return;
        foreach(var leftover in World.GetComponents<CollisionEvent>()){
            World.DetachComponent(leftover);
        }
        var physicsComponents = World.GetComponents<Physics2>().ToList();
        foreach (Physics2 collisionMesh in physicsComponents)
        {
            foreach (Physics2 colliderMesh in physicsComponents)
            {
                if(collisionMesh == colliderMesh || colliderMesh.Z != collisionMesh.Z) continue;
                if(World.QueryComponent<CollisionEvent>(collisionMesh.Owner) != null) break;
                if(World.QueryComponent<CollisionEvent>(colliderMesh.Owner) != null) continue;
                if (CheckMeshCollision(collisionMesh, colliderMesh, out Geometry2D[] colliders))
                {
                    var collisionEvent = World.CreateComponent<CollisionEvent>();
                    collisionEvent.Vertices = colliders;
                    collisionEvent.Collider = colliderMesh.Owner;
                    World.AttachComponents(collisionMesh.Owner, collisionEvent);
                    break;
                };
            }
        }
    }

    private bool CheckMeshCollision(Physics2 mesh1, Physics2 mesh2,out Geometry2D[] colliderVertices)
    {
        colliderVertices = new Geometry2D[2];

        var mesh1Circle = mesh1.CollisionMesh.GetBoundingCircle();
        var mesh2Circle = mesh2.CollisionMesh.GetBoundingCircle();

        if (!Raylib.CheckCollisionCircles(mesh1Circle.Center + mesh1.Position,
                mesh1Circle.Radius,
                mesh2Circle.Center + mesh2.Position,
                mesh2Circle.Radius)
            )
        {
            return false;
        }

        foreach (var vertex1 in mesh1.CollisionMesh.Shapes)
        {
            foreach (var vertex2 in mesh2.CollisionMesh.Shapes)
            {
                if (!vertex1.CollidesWith(this, mesh1, vertex2, mesh2)) continue;
                colliderVertices[0] = vertex1;
                colliderVertices[1] = vertex2;
                return true;
            }
        }
        return false;
    }

    public override void Detach()
    {
        _active = false;
        World.RemoveSystem(this);
    }

    public override void Stop()
    {
        _active = false;
    }

    public override void Initialize()
    {
        _active = true;
    }

    public bool DetectVertexCollision(CircleGeometry circle1, Physics2 pos1, CircleGeometry circle2, Physics2 pos2)
    {
        
        return Raylib.CheckCollisionCircles(
            ApplyRotationMatrix(
                circle1.Offset + pos1.Position,
                pos1.Position,pos1.Rotation),
            circle1.Radius,
            ApplyRotationMatrix(
                circle2.Offset + pos2.Position,
                pos2.Position,pos2.Rotation),
            circle2.Radius);
    }

    public bool DetectVertexCollision(TriangleGeometry triangle, Physics2 pos1, CircleGeometry circle, Physics2 pos2)
    {
        Vector2[] triangleTransformed = new Vector2[3];

        for (int i = 0; i < 3; i++)
        {
            triangleTransformed[i] = ApplyRotationMatrix(
                ApplyRotationMatrix(
                    triangle.Points[i] + triangle.Offset + pos1.Position,
                    triangle.Offset,
                    triangle.Rotation),
                pos1.Position,
                pos1.Rotation
            );
        }

        var circleTransformed = ApplyRotationMatrix(
            circle.Offset + pos2.Position,
            pos2.Position,
            pos2.Rotation
            );

        Vector2 localCircleCenter = circleTransformed - triangleTransformed[0];
        float u = ((triangleTransformed[1].Y - triangleTransformed[2].Y) * localCircleCenter.X + (triangleTransformed[2].X - triangleTransformed[1].X) * localCircleCenter.Y) /
                  ((triangleTransformed[1].Y - triangleTransformed[2].Y) * (triangleTransformed[0].X - triangleTransformed[2].X) + (triangleTransformed[2].X - triangleTransformed[1].X) * (triangleTransformed[0].Y - triangleTransformed[2].Y));

        float v = ((triangleTransformed[2].Y - triangleTransformed[0].Y) * localCircleCenter.X + (triangleTransformed[0].X - triangleTransformed[2].X) * localCircleCenter.Y) /
                  ((triangleTransformed[1].Y - triangleTransformed[2].Y) * (triangleTransformed[0].X - triangleTransformed[2].X) + (triangleTransformed[2].X - triangleTransformed[1].X) * (triangleTransformed[0].Y - triangleTransformed[2].Y));

        float w = 1 - u - v;

        bool insideTriangle = (u >= 0 && u <= 1) && (v >= 0 && v <= 1) && (w >= 0 && w <= 1);
        bool insideCircle = localCircleCenter.Length() <= circle.Radius;

        return insideTriangle || insideCircle;
    }

    public bool DetectVertexCollision(RectangleGeometry rectangleGeometry, Physics2 pos1, CircleGeometry circle, Physics2 pos2)
    {
        var rectangleCenter = rectangleGeometry.Offset + pos1.Position;
        var rectangle = new Rectangle(
            rectangleGeometry.Vertex.x+rectangleGeometry.Offset.X+pos1.Position.X,
            rectangleGeometry.Vertex.y+rectangleGeometry.Offset.Y+pos1.Position.Y,
            rectangleGeometry.Vertex.width,
            rectangleGeometry.Vertex.height
            );
        var circleCenter =
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    circle.Offset + pos2.Position,
                    pos2.Position,
                    pos2.Rotation),
                rectangleCenter,
                -rectangleGeometry.Rotation
            );
        return Raylib.CheckCollisionCircleRec(circleCenter, circle.Radius,rectangle);
    }

    public bool DetectVertexCollision(TriangleGeometry triangle1, Physics2 pos1, TriangleGeometry triangle2, Physics2 pos2)
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

    public bool DetectVertexCollision(RectangleGeometry rect1, Physics2 pos1, RectangleGeometry rect2, Physics2 pos2)
    {
        var rect1Points = rect1.GetRectPoints();
        var rect2Points = rect2.GetRectPoints();
        for (int i = 0; i < 4; i++)
        {
            rect1Points[i] = ApplyRotationMatrix(rect1Points[i]+pos1.Position+rect1.Offset, pos1.Position, pos1.Rotation);
            rect2Points[i] = ApplyRotationMatrix(rect2Points[i]+pos2.Position+rect2.Offset, pos2.Position, pos2.Rotation);
        }

        for (int i = 0; i < 4; i++)
        {
            var edge = rect1Points[(i + 1) % 4] - rect1Points[i];

            var axis = new Vector2(-edge.Y, edge.X);

            float min1 = float.MaxValue, max1 = float.MinValue;
            float min2 = float.MaxValue, max2 = float.MinValue;
            for (int j = 0; j < 4; j++)
            {
                float dotProduct1 = Vector2.Dot(rect1Points[j], axis);
                float dotProduct2 = Vector2.Dot(rect2Points[j], axis);

                min1 = Math.Min(min1, dotProduct1);
                max1 = Math.Max(max1, dotProduct1);
                min2 = Math.Min(min2, dotProduct2);
                max2 = Math.Max(max2, dotProduct2);
            }

            if (!(max1 >= min2 && max2 >= min1))
            {
                return false;
            }
        }

        return true;
    }
    public bool DetectVertexCollision(TriangleGeometry triangle, Physics2 pos1, RectangleGeometry rect, Physics2 pos2)
    {
        Vector2[] triangleTransformed = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            triangleTransformed[i] = ApplyRotationMatrix(
                ApplyRotationMatrix(
                    triangle.Points[i],
                    new Vector2(),
                    triangle.Rotation
                    )+triangle.Offset+pos1.Position,
                pos1.Position,
                pos1.Rotation
            );
        }

        Vector2[] rectangleTransformed =
        {
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    new Vector2(rect.Vertex.x,rect.Vertex.y),
                    Vector2.Zero,
                    rect.Rotation)+rect.Offset+pos2.Position,
                pos2.Position,
                pos2.Rotation
                ),
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    new Vector2(rect.Vertex.x+rect.Vertex.width,rect.Vertex.y),
                    Vector2.Zero,
                    rect.Rotation)+rect.Offset+pos2.Position,
                pos2.Position,
                pos2.Rotation
            ),
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    new Vector2(rect.Vertex.x+rect.Vertex.width,rect.Vertex.y+rect.Vertex.height),
                    Vector2.Zero,
                    rect.Rotation)+rect.Offset+pos2.Position,
                pos2.Position,
                pos2.Rotation
            ),
            ApplyRotationMatrix(
                ApplyRotationMatrix(
                    new Vector2(rect.Vertex.x,rect.Vertex.y+rect.Vertex.height),
                    Vector2.Zero,
                    rect.Rotation)+rect.Offset+pos2.Position,
                pos2.Position,
                pos2.Rotation
            ),
        };
        
        for (int i = 0; i < 5; i++)
        {
            var collisionPoint = new Vector2();
            var endIdx = i < 3 ? i + 1 : 0;
            if (Raylib.CheckCollisionLines(rectangleTransformed[i], rectangleTransformed[endIdx], triangleTransformed[0], triangleTransformed[1],ref collisionPoint) ||
                Raylib.CheckCollisionLines(rectangleTransformed[i], rectangleTransformed[endIdx], triangleTransformed[1], triangleTransformed[2],ref collisionPoint) ||
                Raylib.CheckCollisionLines(rectangleTransformed[i], rectangleTransformed[endIdx], triangleTransformed[2], triangleTransformed[0], ref collisionPoint))
            {
                return true;
            }
        }

        return false;
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
