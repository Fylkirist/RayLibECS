using System.Numerics;
using Raylib_cs;
using RayLibECS.Shapes;
using RayLibECS.Interfaces;
using RayLibECS.Entities;

namespace RayLibECS.Components;

public class RigidBody2 : Component,IBoundingRectable{
    public Shape2D[] Shapes;
    public string Texture;
    public float Mass;
    public float AngularVelocity;
    public RigidBody2(Shape2D[] shapes, string texture, float mass, float angularVelocity){
        Shapes = shapes;
        Mass = mass;
        AngularVelocity = angularVelocity;
        Texture = texture;
    }

    public RigidBody2(){
        Mass = 0f;
        Shapes = new Shape2D[0];
        Texture = "";
        AngularVelocity = 0f;
    }

    public Entity GetOwner(){
        return Owner;
    }

    public Rectangle GetBoundingRect(Vector2 worldPosition){
        float minX = 0f;
        float maxX = 0f;
        float minY = 0f;
        float maxY = 0f;
        for(int i = 0; i<Shapes.Length; i++){
            switch(Shapes[i].Type){
                case ShapeType2D.Triangle:
                    var triangle = Shapes[i].Triangle;
                    foreach (var point in new[] { triangle.P1, triangle.P2, triangle.P3 })
                    {
                        var adjustedX = point.X + Shapes[i].Offset.X;
                        var adjustedY = point.Y + Shapes[i].Offset.Y;
                        minX = Math.Min(minX, adjustedX);
                        maxX = Math.Max(maxX, adjustedX);
                        minY = Math.Min(minY, adjustedY);
                        maxY = Math.Max(maxY, adjustedY);                    
                    }
                    break;

                case ShapeType2D.SymmetricalPolygon:
                    minX = Math.Min(minX,Shapes[i].Offset.X - Shapes[i].SymmetricalPolygon.Radius);
                    maxX = Math.Max(maxX,Shapes[i].Offset.X + Shapes[i].SymmetricalPolygon.Radius);
                    minY = Math.Min(minY,Shapes[i].Offset.Y - Shapes[i].SymmetricalPolygon.Radius);
                    maxY = Math.Max(maxY,Shapes[i].Offset.Y + Shapes[i].SymmetricalPolygon.Radius);
                    break;

                case ShapeType2D.Polygon2:
                    var polygon = Shapes[i].Polygon2;
                    for(int idx = 0; idx<polygon.Vertices.Length; idx++){
                        var adjustedY = polygon.Vertices[idx].Y + Shapes[i].Offset.Y;
                        var adjustedX = polygon.Vertices[idx].X + Shapes[i].Offset.X;

                        minX = Math.Min(adjustedX, minX);
                        maxX = Math.Max(adjustedX, maxX);
                        minY = Math.Min(adjustedY, minY);
                        maxY = Math.Max(adjustedY, maxY);
                    }
                    break;

                case ShapeType2D.Circle:
                    minX = Math.Min(minX,Shapes[i].Offset.X - Shapes[i].Circle.Radius);
                    maxX = Math.Max(maxX,Shapes[i].Offset.X + Shapes[i].Circle.Radius);
                    minY = Math.Min(minY,Shapes[i].Offset.Y - Shapes[i].Circle.Radius);
                    maxY = Math.Max(maxY,Shapes[i].Offset.Y + Shapes[i].Circle.Radius);
                    break;

                case ShapeType2D.Rectangle:
                    var rectangle = Shapes[i].Rectangle;
                    foreach (var point in new[] { rectangle.P1, rectangle.P2, rectangle.P3, rectangle.P4 })
                    {
                        var adjustedX = point.X + Shapes[i].Offset.X;
                        var adjustedY = point.Y + Shapes[i].Offset.Y;
                        minX = Math.Min(minX, adjustedX);
                        maxX = Math.Max(maxX, adjustedX);
                        minY = Math.Min(minY, adjustedY);
                        maxY = Math.Max(maxY, adjustedY);                    
                    }
                    break;
           }
        }
        return new Rectangle(minX+worldPosition.X,minY+worldPosition.Y,maxX-minX,maxY-minY);
    }
}
