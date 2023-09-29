using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private Dictionary<int, InputEvents> _pressedMaps;
    private Dictionary<int, InputEvents> _releasedMaps;
    private string _configFile;

    public InputMapper(string configFile)
    {
        _pressedMaps = new Dictionary<int, InputEvents>();
        _releasedMaps = new Dictionary<int, InputEvents>();
        for (int i = 0; i < _pressedMaps.Count; i++)
        {
            _releasedMaps.Add(i,InputEvents.None);
            _pressedMaps.Add(i, InputEvents.None);
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

    public void SetBinding(InputEvents inputEvent, int key)
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

    public void RemoveBinding(int key)
    {
        _releasedMaps[key] = InputEvents.None;
        _pressedMaps[key] = InputEvents.None;
        SaveKeyMapConfig();
    }

    public void SaveKeyMapConfig()
    {

    }

}