using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

public class MovementSystem : System
{


    public MovementSystem(World world) : base(world)
    {
        
    }


    public override void Initialize()
    {
        var entity = World.CreateEntity("Moveable");
        var moveable = World.CreateComponent<Moveable>();
        var position = World.CreateComponent<Position2>();
        position.Position = Vector2.Zero;
        position.Speed = Vector2.Zero;
        position.Rotation = 0;
        position.RotationSpeed = 0;

        var drawableCircle = World.CreateComponent<DrawableCircle>();
        drawableCircle.Colour = Color.BLUE;
        drawableCircle.Radius = 200;

        World.AttachComponent(entity, drawableCircle);
        World.AttachComponent(entity, position);
        World.AttachComponent(entity, moveable);
    }

    public override void Draw()
    {
        
    }

    public override void Update(float delta, InputState input)
    {
        var moveables = World.GetEntitiesWith<Moveable>();

        foreach (var moveable in moveables)
        {
            foreach (var c in moveable.Components)
            {
                if (c is Position2)
                {
                    var position = (Position2)c;
                    if (input.PressedKeys.Contains(KeyboardKey.KEY_A)) position.Position.X -= 100 * delta;
                    if (input.PressedKeys.Contains(KeyboardKey.KEY_D)) position.Position.X += 100 * delta;
                    if (input.PressedKeys.Contains(KeyboardKey.KEY_W)) position.Position.Y -= 100 * delta;
                    if (input.PressedKeys.Contains(KeyboardKey.KEY_S)) position.Position.Y += 100 * delta;




                }
            }
        }
    }

    public override void Detach()
    {
        
    }


}

public enum MovementType
{
    X,
    Y,
    Z,
    Jumper,
}