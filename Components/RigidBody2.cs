using System.Numerics;

namespace RayLibECS.Components;

public class RigidBody2 : Component{
    public Vector2[] Vertices;
    public RigidBody2(Vector2[] vertices){
        Vertices = vertices;
    }
}
