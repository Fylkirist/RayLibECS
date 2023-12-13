using System.Collections;
using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Events;
using RayLibECS.Shapes;
using RayLibECS.Interfaces;

namespace RayLibECS.Systems;

public class CollisionDetectionSystem : SystemBase{
    private bool _running;
    private double _simulationDistance;
    private bool _renderingWireframes;
    private Dictionary<Entity,Entity> _collision2InCycle;

    public CollisionDetectionSystem(World world, PhysicsMode mode) : base(world)
    {
        _running = false;
        _renderingWireframes = true;
        _collision2InCycle = new Dictionary<Entity, Entity>();
        _simulationDistance = 0;
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Draw()
    {
        if (!_renderingWireframes || !_running) return;
        var camera2 = World.GetComponents<Camera2>().First();
        Raylib.BeginMode2D(camera2.Position);
        var softBodies = World.GetComponents<SoftBody2>();
        var rigidBodies = World.GetComponents<RigidBody2>();
        foreach(var softBody in softBodies){
            var transform = World.QueryComponent<Physics2>(softBody.Owner);
            if(transform == null) continue;
            for(int i = 0; i<softBody.Springs.Length; i++){
                Raylib.DrawLineEx(softBody.Points[softBody.Springs[i].Connection.X].PositionVector + transform.Position,
                        softBody.Points[softBody.Springs[i].Connection.Y].PositionVector + transform.Position,
                        2,
                        Color.WHITE);
            }
            for(int i = 0; i<softBody.Points.Length; i++){
                Raylib.DrawCircleV(softBody.Points[i].PositionVector + transform.Position,softBody.Points[i].Radius,Color.RED); 
            }
            Raylib.DrawText(transform.Position.ToString(), (int)transform.Position.X, (int)transform.Position.Y,24,Color.GREEN);
        }
        
        foreach(var rigidBody in rigidBodies){
            var transform = World.QueryComponent<Physics2>(rigidBody.Owner);
            if(transform == null) continue;
            for(int i = 0; i<rigidBody.Shapes.Length; i++){
                switch(rigidBody.Shapes[i].Type){
                    case ShapeType2D.Rectangle:
                        var rect = rigidBody.Shapes[i].Rectangle;
                        Raylib.DrawLineEx(rect.P1+transform.Position+rigidBody.Shapes[i].Offset,rect.P2+transform.Position+rigidBody.Shapes[i].Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P2+transform.Position+rigidBody.Shapes[i].Offset,rect.P3+transform.Position+rigidBody.Shapes[i].Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P3+transform.Position+rigidBody.Shapes[i].Offset,rect.P4+transform.Position+rigidBody.Shapes[i].Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P4+transform.Position+rigidBody.Shapes[i].Offset,rect.P1+transform.Position+rigidBody.Shapes[i].Offset,2,Color.WHITE);
                        break;
                        
                    case ShapeType2D.SymmetricalPolygon:
                        var symmPoly = rigidBody.Shapes[i].SymmetricalPolygon;
                        Raylib.DrawPolyLinesEx(rigidBody.Shapes[i].Offset + transform.Position,symmPoly.NumVertices,symmPoly.Radius,symmPoly.Rotation*57.5f,2,Color.WHITE);
                        break;
                        
                    case ShapeType2D.Triangle:
                        var triangle = rigidBody.Shapes[i].Triangle;
                        Raylib.DrawTriangleLines(triangle.P1+ rigidBody.Shapes[i].Offset + transform.Position,
                                triangle.P2+ rigidBody.Shapes[i].Offset + transform.Position,
                                triangle.P3+ rigidBody.Shapes[i].Offset + transform.Position,
                                Color.WHITE);
                        break;

                    case ShapeType2D.Circle:
                        var circle = rigidBody.Shapes[i].Circle;
                        Raylib.DrawCircleLines((int)(rigidBody.Shapes[i].Offset.X + transform.Position.X),
                                (int)(rigidBody.Shapes[i].Offset.Y+transform.Position.Y),
                                circle.Radius,
                                Color.WHITE);
                        break;
                    
                    case ShapeType2D.Polygon2:
                        var poly = rigidBody.Shapes[i].Polygon2;
                        for(int vertIdx = 0; vertIdx < poly.NumVertices; vertIdx++){
                            var next = vertIdx + 1 == poly.NumVertices? 0: vertIdx + 1;
                            Raylib.DrawLineEx(poly.Vertices[vertIdx]+transform.Position+ rigidBody.Shapes[i].Offset,
                                    poly.Vertices[next]+transform.Position+ rigidBody.Shapes[i].Offset,
                                    2,
                                    Color.WHITE);
                        }
                        break;
                }
            }
            Raylib.DrawText(transform.Position.ToString(), (int)transform.Position.X, (int)transform.Position.Y,24,Color.GREEN);
        }
        Raylib.EndMode2D();
    }

    public override void Initialize()
    {
        var camEntity = World.CreateEntity("camera");
        var cam = World.CreateComponent<Camera2>();
        cam.Position.zoom = 1f;
        cam.Position.offset = new Vector2(1920 / 2f, 1080 / 2f);
        World.AttachComponents(camEntity,cam);
        _running = true; 
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
        if(!_running) return;
        DetectCollisions2D();
        DetectCollisions3D();
    }

    private void DetectCollisions2D(){
        _collision2InCycle.Clear();
        IOrderedEnumerable<IBoundingRectable> bodies =
            World.GetComponents<SoftBody2>()
            .Concat(World.GetComponents<RigidBody2>().Cast<IBoundingRectable>())
            .Where(e => World.QueryComponent<Physics2>(e.GetOwner())!=null)
            .OrderBy(e => e.GetBoundingRect(World.QueryComponent<Physics2>(e.GetOwner()).Position).x);
        
        var physicalEntities = bodies.Select(e => e.GetOwner()).ToArray();

        foreach(Entity entity1 in physicalEntities){
            foreach(Entity entity2 in physicalEntities){
                if(entity1 == entity2) continue;
                var transform1 = World.QueryComponent<Physics2>(entity1);
                var transform2 = World.QueryComponent<Physics2>(entity2);
                if(transform2 == null) continue;
                if(transform1 == null) break;
                if(transform1.Z != transform2.Z) continue;

                var body1 = World.QueryComponent<RigidBody2>(entity1);
                var body2 = World.QueryComponent<RigidBody2>(entity2);
                
                if(body1 == null){
                    var softBody1 = World.QueryComponent<SoftBody2>(entity1);
                    if(softBody1 == null) break;
                    if(body2 == null){
                        var softBody2 = World.QueryComponent<SoftBody2>(entity2);
                        if(softBody2 == null) continue;
                        if(!DetectSoftBodyCollision(transform1, softBody1,transform2, softBody2)) break;
                        continue;
                    }
                    else{
                        if(!DetectSoftAndRigidBodyCollsion(transform1, softBody1, transform2, body2)) break ;
                        continue;
                    }
                }
                else if(body2 == null){
                    var softBody2 = World.QueryComponent<SoftBody2>(entity2);
                    if(softBody2 == null) continue;
                    if(!DetectSoftAndRigidBodyCollsion(transform2,softBody2,transform1,body1)) break;
                    continue;
                }
                if (!DetectRigidBodyCollsion(transform1, body1, transform2, body2)) break;
            }
        } 
    }

    private bool DetectRigidBodyCollsion(Physics2 entity1Physics, RigidBody2 entity1Body, Physics2 entity2Physics, RigidBody2 entity2Body){

        for(int i = 0; i<entity1Body.Shapes.Length; i++){
            for(int j = 0; j<entity2Body.Shapes.Length; j++){
                float? collided = CheckShapeCollision2(entity1Physics, entity1Body.Shapes[i],entity2Physics, entity2Body.Shapes[j]);
                if(collided != null)
                {
                    if (_collision2InCycle.ContainsKey(entity2Body.Owner) && _collision2InCycle[entity2Body.Owner] == entity1Body.Owner)
                    {
                        return true;
                    }
                    CollisionEvent2 collision =
                        new CollisionEvent2(entity1Body.Owner, i, entity2Body.Owner, j, collided.Value);
                    _collision2InCycle.Add(entity2Body.Owner,entity1Body.Owner);
                    EventBus.Publish(collision);
                    return true;
                }
            }
        }

        return false;
    }

    private Vector3 ToVec3(Vector2 vec){
        return new Vector3(vec.X,vec.Y,0f);
    } 

    private float? CheckShapeCollision2(Physics2 physics1, Shape2D shape1, Physics2 physics2, Shape2D shape2)
    {
        if (shape1.Type == ShapeType2D.Circle && shape2.Type == ShapeType2D.Circle)
        {
            var distance = ((physics1.Position + shape1.Offset) - (physics2.Position + shape2.Offset)).Length();
            return -(distance - shape1.Circle.Radius + shape2.Circle.Radius)>0?-(distance - shape1.Circle.Radius + shape2.Circle.Radius):null;
        }

        var A = ToVec3((physics1.Position + shape1.Offset) - (physics2.Position + shape2.Offset));
        var simplex = new List<Vector3> { A };
        var D = -A;

        while(true){
            A = GetFurthestPoint(shape1,physics1,D) - GetFurthestPoint(shape2,physics2,-D);
            if(Vector3.Dot(A,D)<0){
                return null;
            }

            simplex.Add(A);

            Vector3 a,b,c;
            Vector3 ab,ac,ao;
            Vector3 abPerp,acPerp;

            if(simplex.Count == 2){
                a = simplex[1];
                b = simplex[0];

                ab = b-a;
                ao = -a;
                abPerp = Vector3.Cross(ab,Vector3.Cross(ao,ab));

                D = abPerp;
            }
            else{
                a = simplex[2];
                b = simplex[1];
                c = simplex[0];

                ab = b-a;
                ac = c-a;
                ao = -a;
                
                abPerp = Vector3.Cross(ac,Vector3.Cross(ab,ab));
                acPerp = Vector3.Cross(ab, Vector3.Cross(ac,ac));

                if(Vector3.Dot(abPerp,ao)>0){
                    simplex.Remove(c);
                    D = abPerp;
                }
                else if(Vector3.Dot(acPerp,ao)>0){
                    simplex.Remove(b);
                    D = acPerp;
                }
                else{
                    a = simplex[2];
                    b = simplex[1];
                    c = simplex[0];

                    float distanceA = Vector3.Distance(Vector3.Zero, a);
                    float distanceB = Vector3.Distance(Vector3.Zero, b);
                    float distanceC = Vector3.Distance(Vector3.Zero, c);
 
                    if (distanceA < distanceB && distanceA < distanceC)
                    {
                        return GetClosestPointOnEdge(b, c).Length();
                    }
                    else if (distanceB < distanceA && distanceB < distanceC)
                    {
                        return GetClosestPointOnEdge(a, c).Length();
                    }
                    else
                    {
                        return GetClosestPointOnEdge(a, b).Length();
                    }
                }
            }
        }
    }

    private Vector3 GetClosestPointOnEdge(Vector3 lineStart, Vector3 lineEnd){
        float t = Vector3.Dot(Vector3.Zero - lineStart, lineEnd - lineStart) / Vector3.Dot(lineEnd - lineStart, lineEnd - lineStart);
        t = Math.Max(0, Math.Min(t, 1));
        Vector3 closestPoint = lineStart + t * (lineEnd - lineStart);

        return closestPoint;
    }

    private Vector3 GetFurthestPoint(Shape2D shape, Physics2 transform, Vector3 normal)
    {
        Vector3 transformVec = ToVec3(transform.Position);
        Vector3 offset3 = ToVec3(shape.Offset);
        Vector3 normalized = Vector3.Normalize(normal);

        Vector3 furthest = shape.Type switch
        {
            ShapeType2D.Circle => normalized * shape.Circle.Radius,

            ShapeType2D.Polygon2 =>
                ToVec3(shape.Polygon2.Vertices[GetFurthestPointIndex(shape.Polygon2.Vertices, normal)]),

            ShapeType2D.Triangle =>
                ToVec3(new[] { shape.Triangle.P1, shape.Triangle.P2, shape.Triangle.P3 }
                .MaxBy(v => Vector3.Dot(ToVec3(v),normalized))),

            ShapeType2D.SymmetricalPolygon =>
                GetSymmetricalPolygonSupports(shape.SymmetricalPolygon.NumVertices, shape.SymmetricalPolygon.Rotation,shape.SymmetricalPolygon.Radius)
                    .MaxBy(v => Vector3.Dot(v,normalized)),

            ShapeType2D.Rectangle =>
                ToVec3(new[] { shape.Rectangle.P1, shape.Rectangle.P2, shape.Rectangle.P3, shape.Rectangle.P4 }
                    .MaxBy(v => Vector3.Dot(ToVec3(v),normalized))),

            _ => throw new ArgumentException("Invalid shape type")
        };

        return furthest + transformVec + offset3;
    }

    private int GetFurthestPointIndex(Vector2[] vertices, Vector3 normal)
    {
        int idx = 0;
        var currentAlignment = Vector3.Dot(ToVec3(vertices[0]),normal);

        for (int i = 1; i < vertices.Length; i++)
        {
            var d = Vector3.Dot(ToVec3(vertices[i]),normal);
            if(d > currentAlignment){
                idx = i;
                currentAlignment = d;
            }
        }

        return idx;
    }

    private IEnumerable<Vector3> GetSymmetricalPolygonSupports(int numVertices, float rotation, float radius)
    {
        var startPosition = Vector2.Transform(new Vector2(0, radius), Matrix3x2.CreateRotation(rotation));

        for (int vIdx = 0; vIdx < numVertices; vIdx++)
        {
            var currentVector = Vector2.Transform(startPosition, Matrix3x2.CreateRotation((2 * (float)Math.PI) / numVertices * vIdx));
            yield return ToVec3(currentVector);
        }
    }

    private bool DetectSoftBodyCollision(Physics2 entity1Physics, SoftBody2 entity1Body, Physics2 entity2Physics, SoftBody2 entity2Body){

        for(int i = 0; i<entity1Body.Points.Length; i++){
            for (int j = 0; j<entity2Body.Points.Length; j++){
                var distance = ((entity1Body.Points[i].PositionVector + entity1Physics.Position) - (entity2Body.Points[j].PositionVector + entity2Physics.Position)).Length();
                if(distance < entity1Body.Points[i].Radius + entity2Body.Points[j].Radius){
                    if(!_collision2InCycle.ContainsKey(entity2Body.Owner) || _collision2InCycle[entity2Body.Owner] != entity1Body.Owner){
                        var collision = new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j,0f);
                        _collision2InCycle.Add(entity2Body.Owner,entity1Body.Owner);
                        EventBus.Publish(collision);
                    }
                    return true;
                }
            }
        }

        return false;
    }

    private bool DetectSoftAndRigidBodyCollsion(Physics2 entity1Physics, SoftBody2 entity1Body, Physics2 entity2Physics, RigidBody2 entity2Body){
        var boundingRect1 = entity1Body.GetBoundingRect(entity1Physics.Position);
        var boundingRect2 = entity2Body.GetBoundingRect(entity2Physics.Position);
        if(!Raylib.CheckCollisionRecs(boundingRect1,boundingRect2)) return false;

        return false;
    }

    private void DetectCollisions3D(){
        var physicalEntities = World.GetComponents<Physics3>().ToArray();
        foreach(var collider1 in physicalEntities){
            foreach(var collider2 in physicalEntities){

            }
        }
    }
}
