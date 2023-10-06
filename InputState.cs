using System.Numerics;
using Raylib_cs;

namespace RayLibECS;

public class InputState
{
    private HashSet<KeyboardKey> _pressedKeys;
    public HashSet<KeyboardKey> PressedKeys => _pressedKeys;

    private HashSet<KeyboardKey> _releasedKeys;
    public HashSet<KeyboardKey> ReleasedKeys => _releasedKeys;
    public Vector2 MousePosition { get; private set; }
    public bool IsLeftMousePressed { get; private set; }
    public bool IsLeftMouseReleased { get; private set; }
    public bool IsRightMousePressed { get; private set; }
    public bool IsRightMouseReleased { get; private set; }
    public float MouseWheelMove { get; private set; }

    public InputState()
    {
        _pressedKeys = new HashSet<KeyboardKey>();
        _releasedKeys = new HashSet<KeyboardKey>();
        Update();
    }

    public void Update()
    {
        var newKeys = CreatePressedInputIterator().ToHashSet();
        _releasedKeys.Clear();
        foreach (var key in _pressedKeys)
        {
            if (Raylib.IsKeyDown(key))
            {
                newKeys.Add(key);
            }
            else
            {
                _releasedKeys.Add(key);
            }
        }
        _pressedKeys = newKeys;

        MousePosition = Raylib.GetMousePosition();
        IsLeftMousePressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
        IsLeftMouseReleased = Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
        IsRightMousePressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT);
        IsRightMouseReleased = Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT);
        MouseWheelMove = Raylib.GetMouseWheelMove();
    }

    private IEnumerable<KeyboardKey> CreatePressedInputIterator()
    {
        int current = Raylib.GetKeyPressed();
        while (current != 0)
        {
            yield return (KeyboardKey)current;
            current = Raylib.GetKeyPressed();
        }
    }
}
