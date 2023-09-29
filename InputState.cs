using System.Numerics;
using Raylib_cs;

namespace RayLibECS;

public class InputState
{
    private List<int> _pressedKeys;
    public List<int> PressedKeys => _pressedKeys;
    private List<int> _releasedKeys;
    public List<int> ReleasedKeys => _releasedKeys;
    private Vector2 _mousePosition;
    private CBool _mousePressed;
    private CBool _mouseReleased;
    private float _mouseWheelMove;

    public InputState()
    {
        _pressedKeys = new List<int>();
        _releasedKeys = new List<int>();
        _mousePosition = Raylib.GetMousePosition();
        _mousePressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
        _mouseReleased = new CBool(false);
        _mouseWheelMove = Raylib.GetMouseWheelMove();
    }
    public void Update()
    {
        var newKeys = CreatePressedInputIterator().ToArray();
        var removed = _pressedKeys.Except(newKeys).ToList();
        _pressedKeys = new List<int>(newKeys);
        _releasedKeys = removed;
        _mousePosition = Raylib.GetMousePosition();
    }

    private IEnumerable<int> CreatePressedInputIterator()
    {
        int current = Raylib.GetKeyPressed();
        while (current != 0)
        {
            yield return current;
            current = Raylib.GetKeyPressed();
        }
    }
}