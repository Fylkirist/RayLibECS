using System.Numerics;
using RayLibECS;
using RayLibECS.Ui;
using Raylib_cs;

public class UiWindow : UiElement
{
    private Vector2 _screenPosition;
    private float _width;
    private float _height;
    private string _texture;
    private Color _colour;
    private Color _borderColour;
    private float _borderThickness;
    
    public UiWindow(Vector2 topLeft, float width, float height, string texture, Color colour, Color borderColour, float border)
    {
        _screenPosition = topLeft;
        _width = width;
        _height = height;
        _texture = texture;
        _colour = colour;
        _borderColour = borderColour;
        _borderThickness = border;
    }

    public override void Render(Vector2 parentOffset)
    {
        Raylib.DrawRectangleV(_screenPosition + parentOffset,new Vector2(_width, _height), _colour);
        Raylib.DrawRectangleLinesEx(
                new Rectangle(
                    _screenPosition.X,
                    _screenPosition.Y,
                    _width,
                    _height
                ),
                _borderThickness,
                _borderColour
            );
        foreach(var child in Children){
            child.Render(_screenPosition + parentOffset);
        }
    }

    public override void Update(InputState input, float delta)
    {
        foreach(var child in Children){
            child.Update(input, delta);
        } 
    }
}
