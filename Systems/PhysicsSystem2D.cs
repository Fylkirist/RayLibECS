using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Systems;

internal class PhysicsSystem2D : System
{
    private float _gravity;
    private bool _active;
    public PhysicsSystem2D(World world) : base(world)
    {
        _gravity = 9.82f;
        _active = false;
    }

    public override void Initialize()
    {
        _active = true;
    }

    public override void Draw()
    {
        
    }

    public override void Update(float delta, InputState input)
    {
        
    }

    public override void Detach()
    {
        
    }

    private void HandleCollisions()
    {

    }

    private void HandleGravity()
    {

    }
}