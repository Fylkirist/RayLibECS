using RayLibECS.Events;
using System.Numerics;

namespace RayLibECS.Ui;

public abstract class UiElement{
    protected List<UiElement> Children;
    protected World World;
    
    public delegate void UiAction();

    protected UiElement(){
        Children = new List<UiElement>();
        World = World.Instance;
    }

    protected void PublishUiEvent<T>(T uiEvent) where T: WorldEvent
    {
        World.PublishEvent<T>(uiEvent);
    }

    public abstract void Update(InputState input, float delta);

    public abstract void Render(Vector2 parentOffset);
}
