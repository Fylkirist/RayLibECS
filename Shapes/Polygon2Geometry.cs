using System.Numerics;

namespace RayLibECS.Shapes;


public class Polygon2Geometry : Geometry2D{
    public Polygon Shape;

    public Polygon2Geometry(Vector2 offset, float rotation,  Polygon poly):base(offset, rotation){
        Shape = poly;
    }
}
