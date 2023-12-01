using System.Numerics;
using System.Runtime.InteropServices;

namespace RayLibECS.Shapes;

public enum ShapeType2D{
    Circle,
    Triangle,
    SymmetricalPolygon,
    Rectangle,
    Polygon2
}

public struct Circle
{
    public Vector2 Offset;
    public float Radius; 
}

public struct Triangle{
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;
    public Vector2 Offset;
}

public struct SymmetricalPolygon{
    public int NumVertices;
    public float Radius;
    public float Rotation;
    public Vector2 Offset;
}

public struct BasedRectangle{
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;
    public Vector2 P4;
    public Vector2 Offset;

    public BasedRectangle(float height, float width){
        P1 = new Vector2(-width/2,-height/2);
        P2 = new Vector2(width/2,-height/2);
        P3 = new Vector2(width/2,height/2);
        P4 = new Vector2(-width/2,height/2);
    }
}

public struct Polygon2{
    public Vector2[] Vertices;
    public Vector2 Offset;
    public int NumVertices;

    public Polygon2(Vector2[] vertices, Vector2 offset){
        int numVertices = vertices.Length;
        if(numVertices>16){
            numVertices = 16;
        }
        Vertices = new Vector2[16];
        Offset = offset;
        NumVertices = numVertices;
        for(int i = 0; i<numVertices; i++){
            Vertices[i] = vertices[i];
        }
    }

    public Polygon2(){
        Vertices = new Vector2[16];
        Offset = Vector2.Zero;
        NumVertices = 0;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Shape2D
{
    [FieldOffset(0)]
    public ShapeType2D Type;

    [FieldOffset(4)]
    public Polygon2 Polygon2;

    [FieldOffset(4)]
    public BasedRectangle Rectangle;

    [FieldOffset(4)]
    public SymmetricalPolygon SymmetricalPolygon;

    [FieldOffset(4)]
    public Triangle Triangle;

    [FieldOffset(4)]
    public Circle Circle;
}

