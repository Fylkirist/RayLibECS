using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;
using RayLibECS.Shapes;

namespace RayLibECS.Systems;

internal class RenderingSystem2D : SystemBase
{
    private Entity _currentCamera;
    private bool _active;
    public override void Initialize()
    {
        Initialize2DCamera();
        _active = true;
    }

    public override void Draw()
    {
        if(!_active) return;
        if(!World.IsComponentActive<Camera2>(_currentCamera.Id)) return;
        var activeCamera = World.GetComponents<Camera2>()[_currentCamera.Id];
        Raylib.BeginMode2D(activeCamera.Position);
        var colouredMeshes = World.GetComponents<ColouredMesh2>();
        var animatedSprites = World.GetComponents<AnimatedSprite2>();
        foreach (var VARIABLE in colouredMeshes)
        {
            RenderMesh(VARIABLE);
        }
        
    }

    public override void Update(float delta)
    {
        
        if(!_active) return;
        if (!World.IsComponentActive<Camera2>(_currentCamera.Id)) return;
        var activeCam = World.GetComponents<Camera2>()[_currentCamera.Id];
        foreach (var key in World.InputState.PressedKeys)
        {
            switch (key)
            {
                case KeyboardKey.KEY_RIGHT:
                    activeCam.Position.target.X += delta*1000;
                    break;
                case KeyboardKey.KEY_LEFT:
                    activeCam.Position.target.X -= delta*1000;
                    break;
                case KeyboardKey.KEY_UP:
                    activeCam.Position.target.Y -= delta*1000;
                    break;
                case KeyboardKey.KEY_DOWN:
                    activeCam.Position.target.Y += delta*1000;
                    break;
            }
        }
    }

    public override void Detach()
    {
        _active = false;
        World.DestroyEntity(_currentCamera);
        World.RemoveSystem(this);
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    private void Initialize2DCamera()
    {
        var newCam = World.CreateEntity("camera");
        _currentCamera = newCam;

        var camPosition = World.CreateComponent<Camera2>();
        camPosition.Position = new Camera2D(new Vector2(Raylib.GetScreenWidth()/2,Raylib.GetScreenHeight()/2), new Vector2(0,0), 0, 1);
        World.AttachComponents(newCam,camPosition);
    }

    public RenderingSystem2D(World world) : base(world)
    {
        _currentCamera = new Entity(-1, "Placeholder");
        _active = false;
    }

    private void RenderMesh(ColouredMesh2 mesh)
    {
        if (!World.IsComponentActive<Physics2>(mesh.Owner)) return;
        var position = World.GetComponents<Physics2>()[mesh.Owner];
        for (int i = 0; i < mesh.Mesh.Shapes.Count; i++)
        {
            var shape = mesh.Mesh.Shapes[i];
            Color colour;
            if (mesh.Colours.Count == 0)
            {
                colour = Color.WHITE;
            }
            else
            {
                colour = i < mesh.Colours.Count ? mesh.Colours[i] : mesh.Colours[^1];
            }
            switch (shape)
            {
                case CircleGeometry circle:
                    var circleCenter = Vector2.Transform(position.Position + circle.Offset, Matrix3x2.CreateRotation(position.Rotation, position.Position));
                    Raylib.DrawCircle((int)circleCenter.X,(int)circleCenter.Y,circle.Radius,colour);
                    break;
                case RectangleGeometry rectangle:
                    var rectangleCenter = Vector2.Transform(rectangle.Offset + position.Position,Matrix3x2.CreateRotation(position.Rotation,position.Position));

                    var topLeft = Vector2.Transform(rectangleCenter + rectangle.Vertices[0] + rectangle.Offset,
                        Matrix3x2.CreateRotation(position.Rotation + rectangle.Rotation,rectangleCenter));
                    
                    var topRight = Vector2.Transform(rectangleCenter + rectangle.Vertices[1] + rectangle.Offset,
                        Matrix3x2.CreateRotation(position.Rotation + rectangle.Rotation, rectangleCenter));
                    
                    var bottomRight = Vector2.Transform(rectangleCenter + rectangle.Vertices[2] + rectangle.Offset,
                        Matrix3x2.CreateRotation(position.Rotation + rectangle.Rotation, rectangleCenter));
                    
                    var bottomLeft = Vector2.Transform(rectangleCenter + rectangle.Vertices[3] + rectangle.Offset,
                        Matrix3x2.CreateRotation(position.Rotation + rectangle.Rotation, rectangleCenter));

                    Raylib.DrawTriangle(topLeft,bottomRight,topRight,colour);
                    Raylib.DrawTriangle(bottomLeft,bottomRight ,topLeft , colour);

                    break;
                case TriangleGeometry triangle:
                    var triangleCenter = Vector2.Transform(position.Position + triangle.Offset,
                        Matrix3x2.CreateRotation(position.Rotation,position.Position));

                    var transformedTriangle = new Vector2[3];
                    for (int j = 0; j < triangle.Points.Length; j++)
                    {
                        transformedTriangle[j] = Vector2.Transform(triangle.Points[j]+triangleCenter,Matrix3x2.CreateRotation(triangle.Rotation+position.Rotation,triangleCenter));
                    }
                    Raylib.DrawTriangle(transformedTriangle[0], transformedTriangle[1], transformedTriangle[2],colour);
                    break;
            }
        }
    }
    
    private void RenderSprite(AnimatedSprite2 sprite)
    {
        if(!World.IsComponentActive<Physics2>(sprite.Owner)) return;
        var position = World.GetComponents<Physics2>()[sprite.Owner];
        Raylib.DrawTextureEx(sprite.TextureStateMap[sprite.AnimationState],sprite.Offset+position.Position,position.Rotation,sprite.Scale,sprite.Tint);
    }
}
