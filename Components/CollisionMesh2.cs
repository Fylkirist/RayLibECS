using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Raylib_cs;
using RayLibECS.Systems;
using RayLibECS.Vertices;

namespace RayLibECS.Components;

internal class CollisionMesh2 : Component
{
    public List<Vertex2D> Vertices;
    private Circle? _boundingCircle;
    public float Rotation;
    public CollisionMesh2()
    {
        Vertices = new List<Vertex2D>();
        Rotation = 0;
    }

    public Circle GetBoundingCircle()
    {
        if (_boundingCircle.HasValue) return _boundingCircle.Value;
        var meshCentre = new Vector2(0, 0);
        var meshRadius = 0f;
        foreach (var vertex in Vertices)
        {
            switch (vertex)
            {
                case CircleVertex circle:
                    var distance = Vector2.Distance(meshCentre, circle.Center + circle.Offset) + circle.Radius;
                    if (distance > meshRadius)
                        meshRadius = distance;
                    break;

                case RectangleVertex rectangle:
                    Vector2[] points = new Vector2[4]
                    {
                        new Vector2(rectangle.Vertex.x, rectangle.Vertex.y) + rectangle.Offset,
                        new Vector2(rectangle.Vertex.x + rectangle.Vertex.width, rectangle.Vertex.y) + rectangle.Offset,
                        new Vector2(rectangle.Vertex.x+rectangle.Vertex.width, rectangle.Vertex.y+rectangle.Vertex.width) + rectangle.Offset,
                        new Vector2(rectangle.Vertex.x, rectangle.Vertex.y + rectangle.Vertex.width) + rectangle.Offset
                    };
                    
                    foreach (var vector in points)
                    {
                        var vecLen = Vector2.Distance(meshCentre,
                            CollisionDetectionSystem2D.ApplyRotationMatrix(vector, rectangle.Offset, rectangle.Rotation));
                        if (vecLen > meshRadius) meshRadius = vecLen;
                    }
                    
                    break;

                case TriangleVertex triangle:
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