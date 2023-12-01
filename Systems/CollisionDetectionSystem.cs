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
        }
        
        foreach(var rigidBody in rigidBodies){
            var transform = World.QueryComponent<Physics2>(rigidBody.Owner);
            if(transform == null) continue;
            for(int i = 0; i<rigidBody.Shapes.Length; i++){
                switch(rigidBody.Shapes[i].Type){
                    case ShapeType2D.Rectangle:
                        var rect = rigidBody.Shapes[i].Rectangle;
                        Raylib.DrawLineEx(rect.P1+transform.Position+rect.Offset,rect.P2+transform.Position+rect.Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P2+transform.Position+rect.Offset,rect.P3+transform.Position+rect.Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P3+transform.Position+rect.Offset,rect.P4+transform.Position+rect.Offset,2,Color.WHITE);
                        Raylib.DrawLineEx(rect.P4+transform.Position+rect.Offset,rect.P1+transform.Position+rect.Offset,2,Color.WHITE);
                        break;
                        
                    case ShapeType2D.SymmetricalPolygon:
                        var symmPoly = rigidBody.Shapes[i].SymmetricalPolygon;
                        Raylib.DrawPolyLinesEx(symmPoly.Offset+transform.Position,symmPoly.NumVertices,symmPoly.Radius,symmPoly.Rotation,2,Color.WHITE);
                        break;
                        
                    case ShapeType2D.Triangle:
                        var triangle = rigidBody.Shapes[i].Triangle;
                        Raylib.DrawTriangleLines(triangle.P1+triangle.Offset+transform.Position,
                                triangle.P2+triangle.Offset+transform.Position,
                                triangle.P3+triangle.Offset+transform.Position,
                                Color.WHITE);
                        break;

                    case ShapeType2D.Circle:
                        var circle = rigidBody.Shapes[i].Circle;
                        Raylib.DrawCircleLines((int)(circle.Offset.X+transform.Position.X),
                                (int)(circle.Offset.Y+transform.Position.Y),
                                circle.Radius,
                                Color.WHITE);
                        break;
                    
                    case ShapeType2D.Polygon2:
                        var poly = rigidBody.Shapes[i].Polygon2;
                        for(int vertIdx = 0; vertIdx < poly.NumVertices; vertIdx++){
                            var next = vertIdx + 1 == poly.NumVertices? 0: vertIdx + 1;
                            Raylib.DrawLineEx(poly.Vertices[vertIdx]+transform.Position+poly.Offset,
                                    poly.Vertices[next]+transform.Position+poly.Offset,
                                    2,
                                    Color.WHITE);
                        }
                        break;
                }
            }
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
                if(CheckShapeCollision(entity1Physics, entity1Body.Shapes[i],entity2Physics, entity2Body.Shapes[j])){
                    EventBus.Publish<CollisionEvent2>(new CollisionEvent2(entity1Body.Owner,i,entity2Body.Owner,j));
                    return;
                }
            }
        }
    }

    private bool CheckShapeCollision(Physics2 physics1, Shape2D shape1, Physics2 physics2, Shape2D shape2){
        switch (shape1.Type)
        {
            case ShapeType2D.Circle:
                break;
                
            case ShapeType2D.Polygon2:
                break;

            case ShapeType2D.Triangle:
                break;

            case ShapeType2D.SymmetricalPolygon:
                break;

            case ShapeType2D.Rectangle:
                break;

            default:
                break;
        }

        switch(shape2.Type)
        {
            case ShapeType2D.Circle:
                break;
                
            case ShapeType2D.Polygon2:
                break;

            case ShapeType2D.Triangle:
                break;

            case ShapeType2D.SymmetricalPolygon:
                break;

            case ShapeType2D.Rectangle:
                break;

            default:
                break;
        }
        return true;
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
