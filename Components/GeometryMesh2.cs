using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Shapes;
using RayLibECS.Systems;

namespace RayLibECS.Components;

public class GeometryMesh2
{
    public List<Geometry2D> Shapes;
    private Circle? _boundingCircle;
    public float Rotation;
    public GeometryMesh2()
    {
        Shapes = new List<Geometry2D>();
        Rotation = 0;
    }

    public Circle GetBoundingCircle()
    {
        if (_boundingCircle.HasValue) return _boundingCircle.Value;
        var meshCentre = new Vector2(0, 0);
        var meshRadius = 0f;
        foreach (var vertex in Shapes)
        {
            switch (vertex)
            {
                case CircleGeometry circle:
                    var distance = Vector2.Distance(meshCentre, Vector2.Zero + circle.Offset) + circle.Radius;
                    if (distance > meshRadius)
                        meshRadius = distance;
                    break;

                case RectangleGeometry rectangle:
                    Vector2[] points = new Vector2[4]
                    {
                        rectangle.Vertices[0] + rectangle.Offset,
                        rectangle.Vertices[1] + rectangle.Offset,
                        rectangle.Vertices[2] + rectangle.Offset,
                        rectangle.Vertices[3] + rectangle.Offset
                    };
                    
                    foreach (var vector in points)
                    {
                        var vecLen = Vector2.Distance(meshCentre,
                            CollisionDetectionSystem2D.ApplyRotationMatrix(vector, rectangle.Offset, rectangle.Rotation));
                        if (vecLen > meshRadius) meshRadius = vecLen;
                    }
                    
                    break;

                case TriangleGeometry triangle:
                    foreach (var point in triangle.Points)
                    {
                        var pointDistance = Vector2.Distance(meshCentre, CollisionDetectionSystem2D.ApplyRotationMatrix(point+triangle.Offset,
                            triangle.Offset,triangle.Rotation));
                        if(pointDistance > meshRadius) meshRadius = pointDistance;
                    }   
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        var bCircle = new Circle(meshCentre,meshRadius);
        _boundingCircle = bCircle;
        return bCircle;
    }
}