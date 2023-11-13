using System.Numerics;

namespace RayLibECS.Components;

public class Physics3 : Component{
    public Vector3 Position;
    public Vector3 Velocity;
    public Quaternion Rotation;
    public Quaternion AngularMomentum;
    public PhysicsType3 Type;
    public GeometryMesh3 Mesh;

    public Physics3(){
        Position = Vector3.Zero;
        Velocity = Vector3.Zero;
        Rotation = Quaternion.Zero;
        AngularMomentum = Quaternion.Zero;
        Type = PhysicsType3.Ethereal;
        Mesh = new GeometryMesh3();
    }
}

public enum PhysicsType3{
    Kinematic,
    Ethereal,
    Dynamic,
    Static
}
