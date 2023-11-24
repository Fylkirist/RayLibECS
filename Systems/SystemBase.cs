using RayLibECS.Events;

namespace RayLibECS.Systems;

public abstract class SystemBase
{
    protected World World;

    protected EventBus EventBus;

    protected SystemBase(World world)
    {
        EventBus = EventBus.Instance;
        World = world;
    }
    public abstract void Initialize();

    public abstract void Draw();

    public abstract void Update(float delta);

    public abstract void Detach();
    public abstract void Stop();
}
