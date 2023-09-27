using System.Diagnostics;
using Raylib_cs;

namespace RayLibECS;

internal class Game
{
    private bool _running;
    private InputState _inputState;

    internal Game()
    {
        _running = true;
        _inputState = new InputState();
    }

    public void Initialize()
    {
        Raylib.InitWindow(1920, 1080, "Deez nutz");
        Raylib.SetTargetFPS(144);
        Raylib.InitAudioDevice();

        Task update = Task.Run(UpdateLoop);
        
        Task draw = Task.Run(DrawLoop);

        while (_running)
        {
            if(Raylib.WindowShouldClose())
                _running = false;
        }
        Raylib.CloseWindow();
    }

    public void DrawLoop()
    {
        while (_running)
        {
            Draw();
        }
    }

    public void UpdateLoop()
    {
        var stopwatch = new Stopwatch();
        while (_running)
        {
            Update(stopwatch.ElapsedMilliseconds);
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.BLACK);
        Raylib.DrawCircle(1920 / 2, 1080 / 2, 20, Color.WHITE);
        Raylib.EndDrawing();
    }

    public void Update(long delta)
    {
        _inputState.Update();
    }
}