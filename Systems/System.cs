namespace RayLibECS.Systems;

public abstract class System
{
    protected World _world;

    protected System(World world)
    {
        _world = world;
    }
    public abstract void Initialize();

    public abstract void Draw();

    public abstract void Update(long delta);

    public abstract void Detach();
}