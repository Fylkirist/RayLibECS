using System.Numerics;
using RayLibECS.Systems;

namespace RayLibECS.Components;

public class Physics2:Component
{
    public Vector2 Position;
    public Vector2 Velocity;
    public PhysicsFlagHolder[] PhysicsFlags; 
    public int Z;
    public Physics2()
    {
        Position = new Vector2();
        Velocity = new Vector2();
        PhysicsFlags = new PhysicsFlagHolder[]{
            new(Systems.PhysicsFlags.Collidable,false),
            new(Systems.PhysicsFlags.Movable,false),
            new(Systems.PhysicsFlags.Grounded,false),
            new(Systems.PhysicsFlags.Gravity,false),
            new(Systems.PhysicsFlags.Static,false),
        };

        Z = 0;
    }

    public void SetPhysicsFlags(params KeyValuePair<PhysicsFlags,bool>[] flags){
        foreach(var flag in flags){
            PhysicsFlags[(int)flag.Key].Active = flag.Value;
        }
    }
}


