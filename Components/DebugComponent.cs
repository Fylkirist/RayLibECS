using System.Numerics;

namespace RayLibECS.Components;

public class DebugComponent : Component{
    public float CollisionDistance;
    public Vector2 Position;

    public DebugComponent(){
        CollisionDistance = 0f;
        Position = Vector2.Zero;
    }
}
