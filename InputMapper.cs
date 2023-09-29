using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS;

public enum InputEvents
{
    None = 0,
    OnePressed,
    OneReleased,
    TwoPressed,
    TwoReleased,
    ThreePressed,
    ThreeReleased,
    FourPressed,
    FourReleased,
    FivePressed,
    FiveReleased,
    SixPressed,
    SixReleased,
    SevenPressed,
    SevenReleased,
    EightPressed,
    EightReleased,
    NinePressed,
    NineReleased,
    TenPressed,
    TenReleased,
}

internal class InputMapper
{
    private Dictionary<KeyboardKey, InputEvents> _pressedMaps;
    private Dictionary<KeyboardKey, InputEvents> _releasedMaps;
    private string _configFile;

    public InputMapper(string configFile)
    {
        _pressedMaps = new Dictionary<KeyboardKey, InputEvents>();
        _releasedMaps = new Dictionary<KeyboardKey, InputEvents>();
        for (int i = 0; i < _pressedMaps.Count; i++)
        {
            _releasedMaps.Add(Enum.GetValues<KeyboardKey>()[i],InputEvents.None);
            _pressedMaps.Add(Enum.GetValues<KeyboardKey>()[i], InputEvents.None);
        }
        _configFile = configFile;
        LoadKeyMapConfig();
    }

    public void LoadKeyMapConfig()
    {

    }
    public IEnumerable<InputEvents> GetTokenizedInputs(InputState input)
    {
        foreach (var pressed in input.PressedKeys)
        {
            if (_pressedMaps.TryGetValue(pressed, out var map))yield return map;
        }

        foreach (var released in input.ReleasedKeys)
        {
            if (_releasedMaps.TryGetValue(released, out var map)) yield return map;
        }
    }

    public void SetBinding(InputEvents inputEvent, KeyboardKey key)
    {
        _releasedMaps[key] = inputEvent;
        _pressedMaps[key] = inputEvent;
        
        SaveKeyMapConfig();
    }

    public void RemoveBinding(InputEvents inputEvent)
    {
        var bound = _releasedMaps.FirstOrDefault(e => e.Value == inputEvent).Key;
        _releasedMaps[bound] = InputEvents.None;
        _pressedMaps[bound] = InputEvents.None;
        SaveKeyMapConfig();
    }

    public void RemoveBinding(KeyboardKey key)
    {
        _releasedMaps[key] = InputEvents.None;
        _pressedMaps[key] = InputEvents.None;
        SaveKeyMapConfig();
    }

    public void SaveKeyMapConfig()
    {

    }

}