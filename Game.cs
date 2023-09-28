using System.Diagnostics;
using Raylib_cs;

namespace RayLibECS;

internal class Game
{
    private bool _running;
    private InputState _inputState;
    private World _world;
    internal Game()
    {
        _world = new World();
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
        _world.Draw();
        Raylib.EndDrawing();
    }

    public void Update(long delta)
    {
        Raylib.PollInputEvents();
        _inputState.Update();
        _world.Update(delta,_inputState);
    }
}