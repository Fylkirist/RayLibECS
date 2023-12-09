using System.Numerics;
using Raylib_cs;
using RayLibECS.Interfaces;
using RayLibECS.Entities;

namespace RayLibECS.Components;


public class SoftBody2 : Component, IBoundingRectable{
    public MassPoint2[] Points;
    public Spring[] Springs;
    public float Friction;
    public SoftBody2(MassPoint2[] points, Spring[] springs){
        Points = points;
        Springs = springs;
    }

    public SoftBody2(){
        Points = new MassPoint2[1];
        Springs = new Spring[0];
        Friction = 0f;
    }

    public Entity GetOwner(){
        return Owner;
    }

    public Rectangle GetBoundingRect(Vector2 worldPosition){
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
        return new Rectangle(minX+worldPosition.X,minY+worldPosition.Y,maxX-minX,maxY-minY);
    }
}

public struct MassPoint2
{
    public Vector2 PositionVector;
    public Vector2 VelocityVector;
    public Vector2 ForceVector;
    public float Mass;
    public float Radius;
    
    public MassPoint2(Vector2 position, Vector2 velocity, float mass, float radius){
        ForceVector = Vector2.Zero;
        PositionVector = position;
        VelocityVector = velocity;
        Mass = mass;
        Radius = radius;
    }
}

public struct Spring{
    public Vector2Int Connection;
    public float Stiffness;
    public float RestLength;
    public float Damping;

    public Spring(Vector2Int connection, float stiffness, float rest, float damping)
    {
        Connection = connection;
        Stiffness = stiffness;
        RestLength = rest;
        Damping = damping;
    }
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
