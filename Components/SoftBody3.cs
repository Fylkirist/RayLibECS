using System.Numerics;

namespace RayLibECS.Components;

public class SoftBody3 : Component{
    public MassPoint3[] Points;
    public Spring[] Springs;
    
    public SoftBody3(MassPoint3[] points, Spring[] springs){
        Points = points;
        Springs = springs;
    }
}

public struct MassPoint3{
    public Vector3 PositionVector;
    public Vector3 ForceVector;
    public Vector3 VelocityVector;
    public float Mass;
}
