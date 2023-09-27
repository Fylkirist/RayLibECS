namespace RayLibECS.Systems;

internal abstract class System
{
    public abstract void Initialize();

    public abstract void Draw();

    public abstract void Update();
}