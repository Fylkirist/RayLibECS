namespace RayLibECS.Systems;

public abstract class System
{
    protected World World;

    protected System(World world)
    {
        World = world;
    }
    public abstract void Initialize();

    public abstract void Draw();

    public abstract void Update(float delta,InputState input);

    public abstract void Detach();
}