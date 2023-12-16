using RayLibECS.Components;
using System.Text.Json;

namespace RayLibECS;

public class GameInitializer{
    private World _world;
    
    private void InitializeEntity(InitializationObject[] inits){
        foreach(var init in inits){
            var entity = _world.CreateEntity(init.Tag);
            _world.AttachComponents(entity,init.Components);
        }
    }

    private InitializationObject[] ParseGameFilesJson(string gameFile)
    {
        var inits = new List<InitializationObject>();
        var file = File.OpenRead(gameFile);
        return inits.ToArray();
    }
}

public class InitializationObject{
    public Component[] Components;
    public string Tag;
    
    public InitializationObject(){

    }
}
