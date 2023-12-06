using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Events;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;

public class CollisionDetectionSystem : SystemBase{
    private bool _running;
    private double _simulationDistance;
    private bool _renderingWireframes;

    public CollisionDetectionSystem(World world, PhysicsMode mode) : base(world)
    {
        _running = false;
        _renderingWireframes = true;
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
                        Raylib.DrawLineEx(rect.P2+transform.Position+rigidBody.Shapes[i].Offset, rect.P3+transform.Position+rigidBody.Shapes[i].Offset,2,Color.WHITE);
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
        var physicalEntities = World.GetEntitiesWith<Physics2>().ToArray();
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
                        DetectSoftBodyCollision(transform1, softBody1,transform2, softBody2);
                        continue;
                    }
                    else{
                        DetectSoftAndRigidBodyCollsion(transform1, softBody1, transform2, body2);
                        continue;
                    }
                }
                else if(body2 == null){
                    var softBody2 = World.QueryComponent<SoftBody2>(entity2);
                    if(softBody2 == null) continue;
                    DetectSoftAndRigidBodyCollsion(transform2,softBody2,transform1,body1);
                    continue;
                }
                DetectRigidBodyCollsion(transform1,body1,transform2,body2);
            }
        } 
    }

    private void DetectRigidBodyCollsion(Physics2 entity1Physics, RigidBody2 entity1Body, Physics2 entity2Physics, RigidBody2 entity2Body){
        var boundingRect1 = entity1Body.GetBoundingRect(entity1Physics.Position);
        var boundingRect2 = entity2Body.GetBoundingRect(entity2Physics.Position);
        if(!Raylib.CheckCollisionRecs(boundingRect1,boundingRect2)) return;
        
        for(int i = 0; i<entity1Body.Shapes.Length; i++){
            for(int j = 0; j<entity2Body.Shapes.Length; j++){
                if(CheckShapeCollision2(entity1Physics, entity1Body.Shapes[i],entity2Physics, entity2Body.Shapes[j])){
                    EventBus.Publish(new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j));
                    return;
                }
            }
        }
    }

    private Vector3 ToVec3(Vector2 vec){
        return new Vector3(vec.X,vec.Y,0f);
    }

    private Vector3 GetSupportVector(Shape2D shape1, Physics2 transform1, Shape2D shape2, Physics2 transform2, Vector3 normal)
    {
        return GetFurthestPoint(shape1, transform1, normal) -
               GetFurthestPoint(shape2, transform2, -normal);
    }

    private bool CheckShapeCollision2(Physics2 physics1, Shape2D shape1, Physics2 physics2, Shape2D shape2)
    {
        var epsilon = 1e-6f;

        if (shape1.Type == ShapeType2D.Circle && shape2.Type == ShapeType2D.Circle)
        {
            var distance = ((physics1.Position + shape1.Offset) - (physics2.Position + shape2.Offset)).Length();
            return distance < shape1.Circle.Radius + shape2.Circle.Radius;
        }

        var normal = Vector2.Normalize((physics1.Position + shape1.Offset) - (physics2.Position + shape2.Offset));
        var simplex = new Vector3[3];
        var diff = -simplex[0];

        int maxIterations = 100; 
        int iterationCount = 0;

        while (iterationCount < maxIterations)
        {
            var newSupport = GetSupportVector(shape1, physics1, shape2, physics2, diff);
            if (Vector3.Dot(newSupport, diff) < -epsilon)
            {
                return false;
            }

            simplex[2] = simplex[1];
            simplex[1] = simplex[0];
            simplex[0] = newSupport;

            if (ProcessSimplex(ref simplex, ref diff))
            {
                return true; 
            }

            iterationCount++;
        }

        return false; 
    }

    private bool ProcessSimplex(ref Vector3[] simplex, ref Vector3 diff)
    {
        var a = simplex[0];
        var b = simplex[1];
        var c = simplex[2];

        var AB = b - a;
        var AC = c - a;
        var AO = -a;
        var abp = Vector3.Cross(AC, Vector3.Cross(AB, AB));
        var acp = Vector3.Cross(AB, Vector3.Cross(AC, AC));
        var abpParallel = Vector3.Cross(AC, abp);

        if (abpParallel.LengthSquared() == 0)
        {
            return true;
        }

        if (Vector3.Dot(abp, AO) > 0)
        {
            simplex[2] = a;
            simplex[1] = b;
            diff = abp;
        }
        else if (Vector3.Dot(acp, AO) > 0)
        {
            simplex[2] = a;
            simplex[0] = c;
            diff = acp;
        }
        else
        {

            return true;
        }

        return false;
    }


    private Vector3 GetFurthestPoint(Shape2D shape, Physics2 transform, Vector3 normal)
    {
        Vector3 transformVec = ToVec3(transform.Position);
        Vector3 offset3 = ToVec3(shape.Offset);

        Vector3 furthest = shape.Type switch
        {
            ShapeType2D.Circle => offset3 + normal * shape.Circle.Radius,

            ShapeType2D.Polygon2 =>
                ToVec3(shape.Offset + shape.Polygon2.Vertices[GetFurthestPointIndex(shape.Polygon2.Vertices, normal)] + transform.Position),

            ShapeType2D.Triangle =>
                new[] { shape.Triangle.P1, shape.Triangle.P2, shape.Triangle.P3 }
                    .Select(v => ToVec3(v) + ToVec3(shape.Offset + transform.Position)).MaxBy(v => (v - normal).Length()),

            ShapeType2D.SymmetricalPolygon =>
                GetSymmetricalPolygonSupports(shape.SymmetricalPolygon.NumVertices, shape.SymmetricalPolygon.Rotation, shape.Offset + transform.Position,shape.SymmetricalPolygon.Radius)
                    .MaxBy(v => (v - normal).Length()),

            ShapeType2D.Rectangle =>
                new[] { shape.Rectangle.P1, shape.Rectangle.P2, shape.Rectangle.P3, shape.Rectangle.P4 }
                    .Select(v => ToVec3(v) + ToVec3(shape.Offset + transform.Position)).MaxBy(v => (v - normal).Length()),

            _ => throw new ArgumentException("Invalid shape type")
        };

        return furthest + transformVec;
    }

    private int GetFurthestPointIndex(Vector2[] vertices, Vector3 normal)
    {
        int idx = 0;
        var currentDistance = (ToVec3(vertices[0]) - normal).Length();

        for (int i = 1; i < vertices.Length; i++)
        {
            var d = (ToVec3(vertices[i]) - normal).Length();
            idx = d > currentDistance ? i : idx;
            currentDistance = d;
        }

        return idx;
    }

    private IEnumerable<Vector3> GetSymmetricalPolygonSupports(int numVertices, float rotation, Vector2 offset,float radius)
    {
        var startPosition = Vector2.Transform(new Vector2(0, radius), Matrix3x2.CreateRotation(rotation));

        for (int vIdx = 0; vIdx < numVertices; vIdx++)
        {
            var currentVector = Vector2.Transform(startPosition, Matrix3x2.CreateRotation((2 * (float)Math.PI) / numVertices * vIdx));
            yield return ToVec3(currentVector) + ToVec3(offset);
        }
    }

    private void DetectSoftBodyCollision(Physics2 entity1Physics, SoftBody2 entity1Body, Physics2 entity2Physics, SoftBody2 entity2Body){
        var boundingRect1 = entity1Body.GetBoundingRect(entity1Physics.Position);
        var boundingRect2 = entity2Body.GetBoundingRect(entity2Physics.Position);
        if(!Raylib.CheckCollisionRecs(boundingRect1,boundingRect2)) return;

        for(int i = 0; i<entity1Body.Points.Length; i++){
            for (int j = 0; j<entity2Body.Points.Length; j++){
                var distance = ((entity1Body.Points[i].PositionVector + entity1Physics.Position) - (entity2Body.Points[j].PositionVector + entity2Physics.Position)).Length();
                if(distance < entity1Body.Points[i].Radius + entity2Body.Points[j].Radius){
                    EventBus.Publish(new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j));
                    return;
                }
            }
        }        
    }

    private void DetectSoftAndRigidBodyCollsion(Physics2 entity1Physics, SoftBody2 entity1Body, Physics2 entity2Physics, RigidBody2 entity2Body){
        var boundingRect1 = entity1Body.GetBoundingRect(entity1Physics.Position);
        var boundingRect2 = entity2Body.GetBoundingRect(entity2Physics.Position);
        if(!Raylib.CheckCollisionRecs(boundingRect1,boundingRect2)) return;


    }

    private void DetectCollisions3D(){
        var physicalEntities = World.GetComponents<Physics3>().ToArray();
        foreach(var collider1 in physicalEntities){
            foreach(var collider2 in physicalEntities){

            }
        }
    }
}
