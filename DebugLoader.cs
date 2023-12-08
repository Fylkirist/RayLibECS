using RayLibECS.Components;
using RayLibECS.Shapes;
using System.Numerics;


namespace RayLibECS;

public static class DebugLoader{
    
    public static SoftBody2 CreateSymmetricSoftBody2(int width, int height, float damping, float stiffness, float mass){
        int numPoints = width*height;
        MassPoint2[] points = new MassPoint2[numPoints];
        HashSet<Vector2Int> connections = new HashSet<Vector2Int>();

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; i++){
                var idx = j+1*i;
                var position = new Vector2(
                        -width*10f*0.5f + 10f*i,
                        -height*10f*0.5f + 10f*j
                        );
                var pointMass = mass/(numPoints);
                points[idx] = new MassPoint2(position,Vector2.Zero,pointMass,4);
            }
        }

        for(int i = 0; i < points.Length; i++){
            
        }
        
        List<Spring> springs = new List<Spring>();

        foreach(var elem in connections){
            var spring = new Spring(elem,stiffness,10,damping);
            springs.Add(spring);
        }
        return new SoftBody2(points,springs.ToArray());
    }
}
