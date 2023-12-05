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
                    EventBus.Publish<CollisionEvent2>(new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j));
                    return;
                }
            }
        }
    }

    private Vector3 ToVec3(Vector2 vec){
        return new Vector3(vec.X,vec.Y,0f);
    }

    private bool CheckShapeCollision2(Physics2 physics1, Shape2D shape1, Physics2 physics2, Shape2D shape2){
        if(shape1.Type == ShapeType2D.Circle && shape2.Type == ShapeType2D.Circle){
            var distance = ((physics1.Position + shape1.Offset) - (physics2.Position + shape2.Offset)).Length();
            if(distance < shape1.Circle.Radius + shape2.Circle.Radius){
                return true;
            }
        }
        var normal = ToVec3(Vector2.Normalize((physics1.Position+shape1.Offset) - (physics2.Position+shape2.Offset)));
        var simplex = new List<Vector3>(3);
        simplex.Add(CalculateSupportVector2(shape1,physics1,shape2,physics2,normal));
        var diff = -simplex[0];

        while(true){
            var newSupport = CalculateSupportVector2(shape1,physics1,shape2,physics2, diff);
            if(Vector3.Dot(newSupport,diff) < 0){
                return false;
            }
            Vector3 a,b,c;
            simplex.Add(newSupport);
            if(simplex.Count() == 2){
                b = simplex[0];
                a = simplex[1];
        
                var aBDiff = b-a;
                var aODiff = -a;
                var abPerp = Vector3.Cross(aBDiff, Vector3.Cross(aODiff,aBDiff));
                diff = abPerp;
            }
            else{
                c = simplex[0];
                b = simplex[1];
                a = simplex[2];

                var AB = b-a;
                var AC = c-a;
                var AO = -a;
                var abp = Vector3.Cross(AC,Vector3.Cross(AB,AB));
                var acp = Vector3.Cross(AB,Vector3.Cross(AC,AC));
                if(Vector3.Dot(abp,AO)>0){
                    simplex.Remove(c);
                    diff = abp;
                }
                else if(Vector3.Dot(acp,AO)>0){
                    simplex.Remove(b);
                    diff = acp;
                }
                else{
                    return true;
                }
            }
            
        }
    } 

    private Vector3 CalculateSupportVector2(Shape2D shape1, Physics2 transform1, Shape2D shape2, Physics2 transform2, Vector3 normal){
        var supports = new Vector3[2];

        var shape1Centre = ToVec3(transform1.Position + shape1.Offset);
        var shape2Centre = ToVec3(transform2.Position + shape2.Offset);

        switch (shape1.Type)
        {
            case ShapeType2D.Circle:
                supports[0] = shape1Centre + normal * shape1.Circle.Radius;
                break;
                
            case ShapeType2D.Polygon2:
                int idx = 0;
                var currentDistance = (ToVec3(shape1.Polygon2.Vertices[0]) - normal).Length();
                for(int i = 0; i<shape1.Polygon2.NumVertices; i++){
                    var d = (ToVec3(shape1.Polygon2.Vertices[i])-normal).Length();
                    idx = d<currentDistance ? i: idx;
                }
                supports[0] = ToVec3(shape1.Offset + shape1.Polygon2.Vertices[idx] + transform1.Position);
                break;

            case ShapeType2D.Triangle:
                var currentSupport = ToVec3(shape1.Triangle.P1);
                currentSupport = (currentSupport - normal).Length() > (ToVec3(shape1.Triangle.P2) - normal).Length()? ToVec3(shape1.Triangle.P2): currentSupport;
                currentSupport = (currentSupport - normal).Length() > (ToVec3(shape1.Triangle.P3) - normal).Length()? ToVec3(shape1.Triangle.P3): currentSupport;

                supports[0] = currentSupport + ToVec3(shape1.Offset + transform1.Position);
                break;

            case ShapeType2D.SymmetricalPolygon:
                var startPosition = Vector2.Transform(new Vector2(0,shape1.SymmetricalPolygon.Radius),Matrix3x2.CreateRotation(shape1.SymmetricalPolygon.Rotation));
                var shortestDistance = startPosition;

                for(int vIdx = 0; vIdx<shape1.SymmetricalPolygon.NumVertices; vIdx++){
                    var currentVector = Vector2.Transform(startPosition, Matrix3x2.CreateRotation((2*(float)Math.PI)/shape1.SymmetricalPolygon.NumVertices));
                    
                    var currentVectorDistance = (ToVec3(currentVector) - normal).Length();
                    if(currentVectorDistance < (ToVec3(shortestDistance) - normal).Length()){
                        shortestDistance = currentVector;   
                    }
                }
                supports[0] = ToVec3(shortestDistance + shape1.Offset + transform1.Position);
                break;

            case ShapeType2D.Rectangle:
                var currentRectSupport = ToVec3(shape1.Rectangle.P1);
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape1.Rectangle.P2) - normal).Length()? ToVec3(shape1.Rectangle.P2): currentRectSupport;
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape1.Rectangle.P3) - normal).Length()? ToVec3(shape1.Rectangle.P3): currentRectSupport;
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape1.Rectangle.P4) - normal).Length()? ToVec3(shape1.Rectangle.P4): currentRectSupport;

                supports[0] = currentRectSupport + ToVec3(shape1.Offset + transform1.Position);
                break;

            default:
                break;
        }
        
        normal = -normal;
        switch (shape2.Type)
        {
            case ShapeType2D.Circle:
                supports[1] = shape2Centre + normal * shape2.Circle.Radius;
                break;
                
            case ShapeType2D.Polygon2:
                int idx = 0;
                var currentDistance = (ToVec3(shape2.Polygon2.Vertices[0]) - normal).Length();
                for(int i = 0; i<shape2.Polygon2.NumVertices; i++){
                    var d = (ToVec3(shape2.Polygon2.Vertices[i])-normal).Length();
                    idx = d<currentDistance ? i: idx;
                }
                supports[1] = ToVec3(shape2.Offset + shape2.Polygon2.Vertices[idx] + transform2.Position);
                break;

            case ShapeType2D.Triangle:
                var currentSupport = ToVec3(shape2.Triangle.P1);
                currentSupport = (currentSupport - normal).Length() > (ToVec3(shape2.Triangle.P2) - normal).Length()? ToVec3(shape2.Triangle.P2): currentSupport;
                currentSupport = (currentSupport - normal).Length() > (ToVec3(shape2.Triangle.P3) - normal).Length()? ToVec3(shape2.Triangle.P3): currentSupport;

                supports[1] = currentSupport + ToVec3(shape2.Offset + transform2.Position);
                break;

            case ShapeType2D.SymmetricalPolygon:
                var startPosition = Vector2.Transform(new Vector2(0,shape2.SymmetricalPolygon.Radius),Matrix3x2.CreateRotation(shape2.SymmetricalPolygon.Rotation));

                var shortestDistance = startPosition;

                for(int vIdx = 0; vIdx<shape2.SymmetricalPolygon.NumVertices; vIdx++){
                    var currentVector = Vector2.Transform(startPosition, Matrix3x2.CreateRotation((2*(float)Math.PI)/shape2.SymmetricalPolygon.NumVertices));
                    
                    var currentVectorDistance = (ToVec3(currentVector) - normal).Length();
                    if(currentVectorDistance < (ToVec3(shortestDistance) - normal).Length()){
                        shortestDistance = currentVector;   
                    }
                }
                supports[1] = ToVec3(shortestDistance + shape2.Offset + transform2.Position);
                break;

            case ShapeType2D.Rectangle:
                var currentRectSupport = ToVec3(shape2.Rectangle.P1);
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape2.Rectangle.P2) - normal).Length()? ToVec3(shape2.Rectangle.P2): currentRectSupport;
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape2.Rectangle.P3) - normal).Length()? ToVec3(shape2.Rectangle.P3): currentRectSupport;
                currentRectSupport = (currentRectSupport - normal).Length() > (ToVec3(shape2.Rectangle.P4) - normal).Length()? ToVec3(shape2.Rectangle.P4): currentRectSupport;

                supports[1] = currentRectSupport + ToVec3(shape2.Offset + transform2.Position);
                break;

            default:
                break;
        }

        return Vector3.Normalize(supports[0]-supports[1]);
    }

    private void DetectSoftBodyCollision(Physics2 entity1Physics, SoftBody2 entity1Body, Physics2 entity2Physics, SoftBody2 entity2Body){
        var boundingRect1 = entity1Body.GetBoundingRect(entity1Physics.Position);
        var boundingRect2 = entity2Body.GetBoundingRect(entity2Physics.Position);
        if(!Raylib.CheckCollisionRecs(boundingRect1,boundingRect2)) return;

        for(int i = 0; i<entity1Body.Points.Length; i++){
            for (int j = 0; j<entity2Body.Points.Length; j++){
                var distance = ((entity1Body.Points[i].PositionVector + entity1Physics.Position) - (entity2Body.Points[j].PositionVector + entity2Physics.Position)).Length();
                if(distance < entity1Body.Points[i].Radius + entity2Body.Points[j].Radius){
                    EventBus.Publish<CollisionEvent2>(new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j));
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
