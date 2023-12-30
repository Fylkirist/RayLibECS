using System.Numerics;
using RayLibECS.Shapes;
using Raylib_cs;

namespace RayLibECS.Ui;

public class UiButton : UiElement
{
    public Vector2 _screenPosition;
    public UiAction _onClick;
    public float _width;
    public float _height;
    public Color _colour;

    public UiButton(Vector2 position, UiAction onClick, float width, float height, Color colour)
    {
        _screenPosition = position;
        _onClick = onClick;
        _width = width;
        _height = height;
        _colour = colour;
    }

    public override void Render(Vector2 parentOffset)
    {
        throw new NotImplementedException();
    }

    public override void Update(InputState input, float delta)
    {
        throw new NotImplementedException();
    }
}
