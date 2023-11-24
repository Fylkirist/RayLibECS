using System.Numerics;
using Raylib_cs;

namespace RayLibECS.Components;


public class SoftBody2 : Component{
    public MassPoint2[] Points;
    public Spring[] Springs;
    private Rectangle? _boundingRect;
    public Rectangle BoundingRect => _boundingRect ??= GetBoundingRect();
    public SoftBody2(MassPoint2[] points, Spring[] springs){
        Points = points;
        Springs = springs;
    }

    public SoftBody2(){
        Points = new MassPoint2[1];
        Springs = new Spring[0];
    }

    public Rectangle GetBoundingRect(){
        float minX = Points[0].PositionVector.X;
        float maxX = Points[0].PositionVector.X;
        float minY = Points[0].PositionVector.Y;
        float maxY = Points[0].PositionVector.Y;
        for(int i = 0; i<Points.Length; i++){
            minX = minX>Points[i].PositionVector.X? Points[i].PositionVector.X:minX;
            maxX = maxX<Points[i].PositionVector.X? Points[i].PositionVector.X:maxX;
            minY = minX>Points[i].PositionVector.Y? Points[i].PositionVector.Y:minY;
            maxY = maxY<Points[i].PositionVector.Y? Points[i].PositionVector.Y:maxY;
        }
        return new Rectangle(minX,minY,maxX-minX,maxY-minY);
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
