using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;
using System.Numerics;
using RayLibECS.Events;
using Raylib_cs;

namespace RayLibECS.Systems;

public enum PhysicsMode{
    TWO_DIMENSIONAL,
    THREE_DIMENSIONAL
}

public class PhysicsSystem : SystemBase{
    
    private EventBus _eventBus;
    private float _physicsScale;
    private bool _running;
    private double _simulationDistance;
    private Vector3 _gravityVector; //Temporary solution
    
    public PhysicsSystem(World world, double simDistance, float scale) : base(world){
        _running = false;
        _simulationDistance = simDistance;
        _gravityVector = new Vector3(0,100,0);
        _physicsScale = scale;
        _eventBus = EventBus.Instance;
        _eventBus.Subscribe<CollisionEvent2>(HandleCollision2);
        _eventBus.Subscribe<CollisionEvent3>(HandleCollision3);
    }

    public override void Detach()
    {
        throw new NotImplementedException();
    }

    public override void Draw()
    {
        
    } 

    public override void Initialize(){
        _running = true;
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
        HandleMovement3D(delta);
        HandleMovement2D(delta);
        UpdateSoftBodies2(delta);
        UpdateSoftBodies3(delta);
    }

    public void HandleCollision2(CollisionEvent2 collisionEvent){

        var collider1 = collisionEvent.Collider1;
        var physics1 = World.QueryComponent<Physics2>(collider1);
        if(physics1 == null){
            return;
        }

        var collider2 = collisionEvent.Collider2;
        var physics2 = World.QueryComponent<Physics2>(collider2);
        if(physics2 == null){
            return;
        }

        Console.WriteLine($"Entity: {collider1.Id} is colliding with Entity: {collider2.Id} with overlap: {collisionEvent.Overlap}");
        
        var rigidBody1 = World.QueryComponent<RigidBody2>(collider1);
        var rigidBody2 = World.QueryComponent<RigidBody2>(collider2);

        if(rigidBody1 == null){
            var softBody1 = World.QueryComponent<SoftBody2>(collider1);
            if(softBody1 == null){
                return;
            }
            if(rigidBody2 == null){
                var softBody2 = World.QueryComponent<SoftBody2>(collider2);
                if(softBody2 == null){
                    return;
                }
                SoftBody2Collision(softBody1, physics1, collisionEvent.CollisionIndex1 ,softBody2, physics2, collisionEvent.CollisionIndex2,collisionEvent.Overlap);
                return;
            }   
            else{
                RigidSoftBody2Collision(rigidBody2, physics2,collisionEvent.CollisionIndex1, softBody1, physics1,collisionEvent.CollisionIndex1,collisionEvent.Overlap);
                return;
            }
        }
        if(rigidBody2 == null){
            var softBody2 = World.QueryComponent<SoftBody2>(collider2);
            if(softBody2 == null){
                return;
            }
            RigidSoftBody2Collision(rigidBody1, physics1, collisionEvent.CollisionIndex1, softBody2, physics2,collisionEvent.CollisionIndex2,collisionEvent.Overlap);
            return;
        }

        RigidBody2Collision(rigidBody1,physics1, collisionEvent.CollisionIndex1, rigidBody2,physics2,collisionEvent.CollisionIndex2,collisionEvent.Overlap);

    }

    private void RigidSoftBody2Collision(RigidBody2 rigidBody, Physics2 rigidPhysics, int rigidIndex, SoftBody2 softBody, Physics2 softPhysics, int softIndex, float overlap){
        
    }

    private void SoftBody2Collision(SoftBody2 softBody1, Physics2 physics1, int index1, SoftBody2 softBody2, Physics2 physics2, int index2,float overlap){

    }

    private void RigidBody2Collision(RigidBody2 rigidBody1, Physics2 physics1, int index1, RigidBody2 rigidBody2, Physics2 physics2, int index2, float overlap){
        var distanceVector = (rigidBody1.Shapes[index1].Offset + physics1.Position) - (rigidBody2.Shapes[index2].Offset + physics2.Position);
        var normal = Vector2.Normalize(distanceVector);
        
        physics1.Position += normal * overlap * 0.5f;
        physics2.Position -= normal * overlap * 0.5f;
        
        
    }

    private Vector2 VectorProjection2(Vector2 v, Vector2 u)
    {
        float dotProduct = Vector2.Dot(v, u);
        float magnitudeSquared = u.LengthSquared();
        return (dotProduct / magnitudeSquared) * u;
    }    

    public void HandleCollision3(CollisionEvent3 collisionEvent){

    }

    private Vector2 GetFurthestPoint(Shape2D shape, Physics2 transform, Vector2 normal)
    {
        Vector2 transformVec = transform.Position;
        Vector2 offset = shape.Offset;
        Vector2 normalized = Vector2.Normalize(normal);

        Vector2 furthest = shape.Type switch
        {
            ShapeType2D.Circle => normalized * shape.Circle.Radius,

            ShapeType2D.Polygon2 => shape.Polygon2.Vertices[GetFurthestPointIndex(shape.Polygon2.Vertices, normal)],

            ShapeType2D.Triangle =>
                new[] { shape.Triangle.P1, shape.Triangle.P2, shape.Triangle.P3 }
                    .MaxBy(v => Vector2.Dot(v,normalized)),

            ShapeType2D.SymmetricalPolygon =>
                GetSymmetricalPolygonSupports(shape.SymmetricalPolygon.NumVertices, shape.SymmetricalPolygon.Rotation,shape.SymmetricalPolygon.Radius)
                    .MaxBy(v => Vector2.Dot(v, normalized)),

            ShapeType2D.Rectangle =>
                new[] { shape.Rectangle.P1, shape.Rectangle.P2, shape.Rectangle.P3, shape.Rectangle.P4 }
                    .MaxBy(v => Vector2.Dot(v, normalized)),

            _ => throw new ArgumentException("Invalid shape type")
        };

        return furthest + transformVec + offset;
    }

    private int GetFurthestPointIndex(Vector2[] vertices, Vector2 normal)
    {
        int idx = 0;
        var currentDistance = Vector2.Dot(vertices[0],normal);

        for (int i = 1; i < vertices.Length; i++)
        {
            var d = Vector2.Dot(vertices[i],normal);
            if (d > currentDistance)
            {
                idx = i;
                currentDistance = d;
            }
        }

        return idx;
    }

    private IEnumerable<Vector2> GetSymmetricalPolygonSupports(int numVertices, float rotation,float radius)
    {
        var startPosition = Vector2.Transform(new Vector2(0, radius), Matrix3x2.CreateRotation(rotation));

        for (int vIdx = 0; vIdx < numVertices; vIdx++)
        {
            var currentVector = Vector2.Transform(startPosition, Matrix3x2.CreateRotation((2 * (float)Math.PI) / numVertices * vIdx));
            yield return currentVector;
        }
    }

    private void UpdateSoftBodies2(float delta){
        var softBodies2 = World.GetComponents<SoftBody2>();
        foreach(SoftBody2 softBody in softBodies2){
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].ForceVector.X = _gravityVector.X * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Y = _gravityVector.Y * softBody.Points[i].Mass; 
            }
            for(int i = 0; i<softBody.Springs.Length; i++){
                var currentSpringLength = (softBody.Points[softBody.Springs[i].Connection.X].PositionVector -
                        softBody.Points[softBody.Springs[i].Connection.Y].PositionVector).Length();
                var lengthdiff = currentSpringLength - softBody.Springs[i].RestLength;
                var massPointNormal = Vector2.Normalize(softBody.Points[softBody.Springs[i].Connection.X].PositionVector - softBody.Points[softBody.Springs[i].Connection.Y].PositionVector);
                var velocityDifference = softBody.Points[softBody.Springs[i].Connection.X].VelocityVector - softBody.Points[softBody.Springs[i].Connection.Y].VelocityVector;
                var springForce = (Vector2.Dot(massPointNormal,velocityDifference) * softBody.Springs[i].Damping) + lengthdiff * softBody.Springs[i].Stiffness;

                var springForceVector = springForce * massPointNormal;
                
                softBody.Points[softBody.Springs[i].Connection.X].ForceVector += springForceVector;
                softBody.Points[softBody.Springs[i].Connection.Y].ForceVector -= springForceVector;
            }
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].VelocityVector.X += (softBody.Points[i].ForceVector.X * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Y += (softBody.Points[i].ForceVector.Y * delta)/softBody.Points[i].Mass;
                softBody.Points[i].PositionVector += softBody.Points[i].VelocityVector;
            }
        }        
    }

    private void UpdateSoftBodies3(float delta){
        var softBodies3 = World.GetComponents<SoftBody3>();
        foreach(SoftBody3 softBody in softBodies3){
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].ForceVector.X = _gravityVector.X * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Y = _gravityVector.Y * softBody.Points[i].Mass;
                softBody.Points[i].ForceVector.Z = _gravityVector.Z * softBody.Points[i].Mass;
            }
            for(int i = 0; i<softBody.Springs.Length; i++){
                var currentSpringLength = (softBody.Points[softBody.Springs[i].Connection.X].PositionVector -
                        softBody.Points[softBody.Springs[i].Connection.Y].PositionVector).Length();
                var lengthdiff = currentSpringLength - softBody.Springs[i].RestLength;
                var massPointNormal = Vector3.Normalize(softBody.Points[softBody.Springs[i].Connection.X].PositionVector - softBody.Points[softBody.Springs[i].Connection.Y].PositionVector);
                var velocityDifference = softBody.Points[softBody.Springs[i].Connection.X].VelocityVector - softBody.Points[softBody.Springs[i].Connection.Y].VelocityVector;
                var springForce = (Vector3.Dot(massPointNormal,velocityDifference) * softBody.Springs[i].Damping) + lengthdiff * softBody.Springs[i].Stiffness;

                var springForceVector = springForce * massPointNormal;
                
                softBody.Points[softBody.Springs[i].Connection.X].ForceVector += springForceVector;
                softBody.Points[softBody.Springs[i].Connection.Y].ForceVector -= springForceVector;
            }
            for(int i = 0; i<softBody.Points.Length; i++){
                softBody.Points[i].VelocityVector.X += (softBody.Points[i].ForceVector.X * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Y += (softBody.Points[i].ForceVector.Y * delta)/softBody.Points[i].Mass;
                softBody.Points[i].VelocityVector.Z += (softBody.Points[i].ForceVector.Z * delta)/softBody.Points[i].Mass;
                softBody.Points[i].PositionVector += softBody.Points[i].VelocityVector;
            }
        }
    }

    public void HandleMovement2D(float delta){
       var physicsComponents = World.GetComponents<Physics2>();
       foreach(var component in physicsComponents){
           component.Position += component.Velocity * delta;
           var rigidBody = World.QueryComponent<RigidBody2>(component.Owner);
           if(rigidBody != null){
               UpdateRigidBody2(rigidBody, component, delta);
           }
       }
    }

    private void UpdateRigidBody2(RigidBody2 body, Physics2 transform, float delta){
        var rotationDiff = body.AngularVelocity*delta;
        for(int i = 0; i<body.Shapes.Length; i++){
            switch(body.Shapes[i].Type){
                case ShapeType2D.SymmetricalPolygon:
                    body.Shapes[i].SymmetricalPolygon.Rotation += rotationDiff;
                    if(body.Shapes[i].SymmetricalPolygon.Rotation > 2*Math.PI){
                        body.Shapes[i].SymmetricalPolygon.Rotation -= 2*(float)Math.PI;
                    }
                    body.Shapes[i].Offset = Vector2.Transform(body.Shapes[i].Offset,Matrix3x2.CreateRotation(rotationDiff));
                    break;
                    
                case ShapeType2D.Circle:
                    body.Shapes[i].Offset = Vector2.Transform(body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));
                    break;

                case ShapeType2D.Polygon2:
                    for(int vIdx = 0; vIdx<body.Shapes[i].Polygon2.NumVertices; vIdx++){
                        body.Shapes[i].Polygon2.Vertices[vIdx] =
                            Vector2.Transform(body.Shapes[i].Polygon2.Vertices[vIdx],Matrix3x2.CreateRotation(rotationDiff));
                        body.Shapes[i].Polygon2.Vertices[vIdx] = 
                            Vector2.Transform(body.Shapes[i].Polygon2.Vertices[vIdx]+body.Shapes[i].Offset,Matrix3x2.CreateRotation(rotationDiff));
                    }
                    body.Shapes[i].Offset = Vector2.Transform(body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));
                    break;

                case ShapeType2D.Triangle:
                    body.Shapes[i].Triangle.P1 = Vector2.Transform(body.Shapes[i].Triangle.P1, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Triangle.P1 = Vector2.Transform(body.Shapes[i].Triangle.P1+body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Triangle.P2 = Vector2.Transform(body.Shapes[i].Triangle.P2, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Triangle.P2 = Vector2.Transform(body.Shapes[i].Triangle.P2+body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Triangle.P3 = Vector2.Transform(body.Shapes[i].Triangle.P3, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Triangle.P3 = Vector2.Transform(body.Shapes[i].Triangle.P3+body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Offset = Vector2.Transform(body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));
                    break;

                case ShapeType2D.Rectangle:
                    body.Shapes[i].Rectangle.P1 = Vector2.Transform(body.Shapes[i].Rectangle.P1, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Rectangle.P1 = Vector2.Transform(body.Shapes[i].Rectangle.P1 + body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Rectangle.P2 = Vector2.Transform(body.Shapes[i].Rectangle.P2, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Rectangle.P2 = Vector2.Transform(body.Shapes[i].Rectangle.P2 + body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Rectangle.P3 = Vector2.Transform(body.Shapes[i].Rectangle.P3, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Rectangle.P3 = Vector2.Transform(body.Shapes[i].Rectangle.P3 + body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Rectangle.P4 = Vector2.Transform(body.Shapes[i].Rectangle.P4, Matrix3x2.CreateRotation(rotationDiff));
                    body.Shapes[i].Rectangle.P4 = Vector2.Transform(body.Shapes[i].Rectangle.P4 + body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));

                    body.Shapes[i].Offset = Vector2.Transform(body.Shapes[i].Offset, Matrix3x2.CreateRotation(rotationDiff));
                    break;

                default:
                    break;
            }
        }
    }

    public void HandleMovement3D(float delta){
        var physicsComponents = World.GetComponents<Physics3>();
        foreach (var component in physicsComponents)
        {
           component.Position += component.Velocity * delta;
           component.Rotation += component.AngularMomentum * delta;
        }
    }
}
