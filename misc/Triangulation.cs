using System.Numerics;
using RayLibECS.Shapes;

public static class TriangulationTest
{

    public static Triangle[] TriangulatePolygon(int numContours, int[] counter, Vector2[] vertices)
    {
        List<Triangle> triangles = new List<Triangle>();
        Segment[] segments = new Segment[vertices.Length];
        int nmonpoly;
        int cCount = 0;
        int i = 1;
        
        while(cCount < numContours){
            int numPoints = counter[cCount];
            int first = i;
            int last = first + numPoints - 1;
            
            for(int j = 0; j < numPoints; j++, i++){
                segments[i].v0 = vertices[i];

                if(i == last){
                    segments[i].next = first;
                    segments[i].previous = i - 1;
                    segments[i - 1].v1 = segments[i].v0;
                }
                else if(i == first){
                    segments[i].next = i + 1;
                    segments[i].previous = last;
                    segments[last].v1 = segments[i].v0;
                }
                else{
                    segments[i].next = i + 1;
                    segments[i].previous = i - 1;
                    segments[i-1].v1 = segments[i].v0;
                }
                segments[i].isInserted = false;
            }

            cCount++;
        }

        int genus = numContours - 1;
        int n = i - 1;
        
        initialize(ref segments);
        constructTrapezoids(ref segments);


        return triangles.ToArray();
    }

    public static void initialize(ref Segment[] segments){
        Random rand = new Random();

        for (int i = segments.Length - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);

            Segment temp = segments[i];
            segments[i] = segments[j];
            segments[j] = temp;
        }
    }

    public static void constructTrapezoids(ref Segment[] segments){
        
    }
}


public struct Segment{
    public Vector2 v0,v1;
    public int next;
    public int previous;
    public bool isInserted;
    public int root0, root1;
}

public struct Trapezoid{
    public int leftSegment, rightSegment;
    public Vector2 high, low;
    public int u0, u1;
    public int d0, d1;
    public int sink;
    public int uSave, uSide;
    public bool stateValid;
}

public class QueryNode{
    public bool Type;
    public int SegmentNumber;
    public Vector2 YValue;
    public int TrNum;
    public QueryNode? Parent;
    public QueryNode? Left, Right;

    public QueryNode(bool t, int segNum, Vector2 yVal, int tr, QueryNode? parent = null, QueryNode? left = null, QueryNode? right = null)
    {
        Type = t;
        SegmentNumber = segNum;
        YValue = yVal;
        TrNum = tr;
        Parent = parent;
        Left = left;
        Right = right;
    }
}
