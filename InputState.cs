using System.Numerics;
using Raylib_cs;

namespace RayLibECS;

public class InputState
{
    private LinkedList<KeyboardKey> _pressedKeys;
    private LinkedList<KeyboardKey> _releasedKeys;
    private Vector2 _mousePosition;
    private CBool _mousePressed;
    private CBool _mouseReleased;
    private float _mouseWheelMove;
    public void Update()
    {
        
    }
}