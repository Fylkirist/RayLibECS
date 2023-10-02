using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Systems;
using RayLibECS.Vertices;

namespace RayLibECS.Components;

internal class CollisionMesh2 : Component
{
    public List<Vertex2D> Vertices;
    private Circle? _boundingCircle;
    public CollisionMesh2()
    {
        Vertices = new List<Vertex2D>();
    }

    public Circle GetBoundingCircle()
    {
        if (_boundingCircle.HasValue) return _boundingCircle.Value;
        var max = new Vector2();
        var min = new Vector2();
        foreach (var vertex in Vertices)
        {
            switch (vertex)
            {
                case CircleVertex circle:
                    var centre = circle.Center + circle.Offset;
                    min.X = centre.X-circle.Radius > min.X ?
                        min.X :
                        centre.X-circle.Radius;

                    min.Y = centre.Y - circle.Radius > min.Y ?
                        min.Y :
                        centre.Y-circle.Radius;

                    max.X = centre.X + circle.Radius < max.X ?
                        max.X :
                        centre.X + circle.Radius;

                    max.Y = centre.Y + circle.Radius < max.Y ?
                        max.Y :
                        centre.Y + circle.Radius;
                    break;
                case RectangleVertex rectangle:
                    min.X = min.X > rectangle.Vertex.x + rectangle.Offset.X
                        ? rectangle.Vertex.x + rectangle.Offset.X
                        : min.X;

                    min.Y = min.Y > rectangle.Vertex.y + rectangle.Offset.Y
                        ? rectangle.Vertex.y + rectangle.Offset.Y
                        : min.Y;

                    max.X = max.X < rectangle.Vertex.x + rectangle.Offset.X?
                        rectangle.Vertex.x + rectangle.Offset.X:
                        max.X;

                    max.Y = max.Y < rectangle.Vertex.y + rectangle.Offset.Y ?
                        rectangle.Vertex.y + rectangle.Offset.Y:
                        max.Y;
                    break;
                case TriangleVertex triangle:
                    foreach (var point in triangle.Points)
                    {
                        min.X = min.X > point.X + triangle.Offset.X
                            ? point.X + triangle.Offset.X
                            : min.X;

                        min.Y = min.Y > point.Y + triangle.Offset.Y
                            ? point.Y + triangle.Offset.Y
                            : min.Y;

                        max.X = max.X < point.X + triangle.Offset.X ?
                            point.X + triangle.Offset.X :
                            max.X;

                        max.Y = max.Y < point.Y + triangle.Offset.Y ?
                            point.Y + triangle.Offset.Y :
                            max.Y;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        var rect = new Circle();
        _boundingCircle = rect;
        return rect;
    }
}