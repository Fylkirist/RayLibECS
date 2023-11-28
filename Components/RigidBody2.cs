using System.Numerics;
using Raylib_cs;
using RayLibECS.Shapes;

namespace RayLibECS.Components;

public class RigidBody2 : Component{
    public Geometry2D[] Shapes;
    public string Texture;
    public float Mass;
    public float AngularVelocity;
    public RigidBody2(Geometry2D[] shapes, string texture, float mass, float angularVelocity){
        Shapes = shapes;
        Mass = mass;
        AngularVelocity = angularVelocity;
        Texture = texture;
    }

    public RigidBody2(){
        Mass = 0f;
        Shapes = new Geometry2D[0];
        Texture = "";
        AngularVelocity = 0f;
    }

    public Rectangle GetBoundingRect(){
        float minX = 0f;
        float maxX = 0f;
        float minY = 0f;
        float maxY = 0f;
        for(int i = 0; i<Shapes.Length; i++){
           switch(Shapes[i]){
                case TriangleGeometry triangle:
                    break;
                case RectangleGeometry rectangle:
                    break;
                case CircleGeometry circle:
                    break;
                case Polygon2Geometry polygon:
                    break;
           }
        }
        return new Rectangle(minX,minY,maxX-minX,maxY-minY);
    }
}
