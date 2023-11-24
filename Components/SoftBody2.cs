using System.Numerics;

namespace RayLibECS.Components;


public class SoftBody2 : Component{
    public MassPoint2[] Points;
    public Spring[] Springs;
    public SoftBody2(MassPoint2[] points, Spring[] springs){
        Points = points;
        Springs = springs;
    }

    public SoftBody2(){
        Points = new MassPoint2[1];
        Springs = new Spring[0];
    }
}

public struct MassPoint2
{
    public Vector2 PositionVector;
    public Vector2 VelocityVector;
    public Vector2 ForceVector;
    public float Mass;
}

public struct Spring{
    public Vector2Int Connection;
    public float Stiffness;
    public float RestLength;
    public float Damping;
}

public struct Vector2Int{
    public int X;
    public int Y;
    
    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }
    public static Vector2Int operator +(Vector2Int a, Vector2Int b){
        return new Vector2Int(a.X+b.X,a.Y+b.Y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b){
        return new Vector2Int(a.X-b.X,a.Y-b.Y);
    }
}
