using Raylib_cs;
using System.Numerics;

namespace RayLibECS.Components;

public class GeometryMesh3{
    public Mesh Mesh;
    public Vector3 Offset;
    public Quaternion Rotation;

    public GeometryMesh3()
    {
        Mesh = new Mesh();
        Offset = Vector3.Zero;
        Rotation = Quaternion.Zero;
    }
}
