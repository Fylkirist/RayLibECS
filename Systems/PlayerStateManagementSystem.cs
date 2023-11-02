using RayLibECS.Interfaces;

namespace RayLibECS.Systems;


public class EntityStateManagementSystem:SystemBase
{

    private Dictionary<string,IStateFactory> _factoryDict;

    public EntityStateManagementSystem(World world):base(world)
    {
        _factoryDict = new Dictionary<string, IStateFactory>();
    }

    public override void Draw()
    {

    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }

    public override void Update(float delta)
    {
            
    }

    public override void Detach()
    {
            
    }

    public override void Initialize()
    {

    }
}
