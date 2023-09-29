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
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        _world.InitializeWorld();

        Task update = Task.Run(UpdateLoop);

        while (_running)
        {
            Draw();
        }
        Raylib.CloseWindow();
    }


    public void UpdateLoop()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        long previousTime = stopwatch.ElapsedMilliseconds;

        while (_running)
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            float deltaTime = (currentTime - previousTime) / 1000f; // Convert to seconds

            Update(deltaTime);

            previousTime = currentTime;
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.DrawFPS(100,100);
        _world.Draw();
        Raylib.EndDrawing();
    }

    public void Update(float delta)
    {
        _inputState.Update();
        if (Raylib.WindowShouldClose()) _running = false;
        _world.Update(delta,_inputState);
    }
}