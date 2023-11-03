namespace RayLibECS.Components;

public class RenderableComponent:Component
{
    public RenderingModes RenderingMode;
    public int Z;

    public RenderableComponent()
    {
        RenderingMode = RenderingModes.TwoD;
        Z = 0;
    }
}