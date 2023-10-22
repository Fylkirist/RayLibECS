using System.Numerics;
using RayLibECS.Systems;

namespace RayLibECS.Components;

public class Physics2:Component
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;
    public float RotationSpeed;
    public float Mass;
    public PhysicsType2D PhysicsType;
    public GeometryMesh2 CollisionMesh;
    public float Absorbtion;
    public float Friction;
    public int Z;

    public Physics2()
    {
        Position = new Vector2();
        Velocity = new Vector2();
        Rotation = 0f;
        RotationSpeed = 0f;
        Mass = 0f;
        PhysicsType = PhysicsType2D.Dynamic;
        CollisionMesh = new GeometryMesh2();
        Absorbtion = 0f;
        Friction = 0f;
        Z = 0;
    }
}
