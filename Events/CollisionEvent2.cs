using RayLibECS.Entities;

namespace RayLibECS.Events;

public class CollisionEvent2{
    public Entity Collider1;
    public Entity Collider2;
    public int CollisionIndex1;
    public int CollisionIndex2;

    public CollisionEvent2(Entity c1, int index1, Entity c2, int index2){
        Collider1 = c1;
        CollisionIndex1 = index1;
        Collider2 = c2;
        CollisionIndex2 = index2;
    }
}
