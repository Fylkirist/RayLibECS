using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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

    public Physics2()
    {
        Position = new Vector2();
        Velocity = new Vector2();
        Rotation = 0;
        RotationSpeed = 0;
        Mass = 0;
        PhysicsType = PhysicsType2D.Dynamic;
        CollisionMesh = new GeometryMesh2();
        Absorbtion = 0;
    }
}