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
    public string Texture;
    public bool Hovering;

    public UiButton(Vector2 position, UiAction onClick, float width, float height, Color colour, string texture)
    {
        _screenPosition = position;
        _onClick = onClick;
        _width = width;
        _height = height;
        _colour = colour;
        Texture = texture;
        Hovering = false;
    }

    public override void Render(Vector2 parentOffset)
    {
        var buttonTexture = World.AssetManager.GetTexture(Texture);
        Raylib.DrawRectangleV(_screenPosition,new Vector2(_width,_height),_colour);
        Raylib.DrawTextureV(buttonTexture, _screenPosition, _colour);
    }

    public override void Update(InputState input, float delta)
    {
        var mouseX = input.MousePosition.X;
        var mouseY = input.MousePosition.Y;

        if(mouseX > _screenPosition.X 
            && mouseX < _screenPosition.X + _width 
            && mouseY > _screenPosition.Y 
            && mouseY < _screenPosition.Y + _height)
        {
            Hovering = true;
        }
        else
        {
            Hovering = false;
        }
        if(Hovering && input.IsLeftMouseReleased)
        {
            _onClick.Invoke();
        }
    }
}
