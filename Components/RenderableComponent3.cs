using Raylib_cs;
using System.Numerics;

namespace RayLibECS.Components;

public class RenderableComponent3 : Component{
    public GeometryMesh3 Mesh;
    public Material Material;
    public MaterialMap Map;

    public RenderableComponent3(){
        Mesh = new GeometryMesh3();
        Material = new Material();
        Map = new MaterialMap();
    }
}
