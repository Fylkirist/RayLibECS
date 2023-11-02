using System.Numerics;
using Raylib_cs;
using RayLibECS.Components;
using RayLibECS.Entities;

namespace RayLibECS.Systems;

public class MovementSystem : SystemBase
{


    public MovementSystem(World world) : base(world)
    {
        
    }


    public override void Initialize()
    {
        World.AllocateComponentArray<Moveable>();
    }

    public override void Draw()
    {
        
    }

    public override void Stop()
    {

    }

    public override void Update(float delta)
    {
        var moveables = World.GetEntitiesWith<Moveable>();
        var input = World.InputState;
        foreach (var moveable in moveables)
        {
            var position = World.QueryComponent<Physics2>(moveable);
            if (position == null) continue;
                if (input.PressedKeys.Contains(KeyboardKey.KEY_A)) position.Position.X -= 100 * delta;
                if (input.PressedKeys.Contains(KeyboardKey.KEY_D)) position.Position.X += 100 * delta;
                if (input.PressedKeys.Contains(KeyboardKey.KEY_W)) position.Position.Y -= 100 * delta;
                if (input.PressedKeys.Contains(KeyboardKey.KEY_S)) position.Position.Y += 100 * delta;
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
