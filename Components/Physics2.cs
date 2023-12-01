using System.Numerics;
using RayLibECS.Systems;

namespace RayLibECS.Components;

public class Physics2:Component
{
    public Vector2 Position;
    public Vector2 Velocity;
    public PhysicsType2D PhysicsType; 
    public int Z;
    public Physics2()
    {
        Position = new Vector2();
        Velocity = new Vector2();
        PhysicsType = PhysicsType2D.Dynamic;       
        Z = 0;
    }
}

public enum PhysicsType2D{
    Static,
    Ethereal,
    Dynamic,
    Kinematic,
}
