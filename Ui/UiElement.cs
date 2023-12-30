using RayLibECS.Events;

namespace RayLibECS.Ui;

public abstract class UiElement{
    protected List<UiElement> Children;
    protected World World;
    
    protected UiElement(){
        Children = new List<UiElement>();
        World = World.Instance;
    }

    protected void PublishUiEvent<T>(T uiEvent) where T: WorldEvent
    {
        World.PublishEvent<T>(uiEvent);
    }

    protected abstract void Update(InputState input, float delta);

    protected abstract void Render();
}
