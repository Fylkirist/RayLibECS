﻿namespace RayLibECS;

internal class ComponentManager
{
    private Dictionary<Type, HashSet<int>> _liveComponents;

    internal ComponentManager()
    {
        _liveComponents = new Dictionary<Type, HashSet<int>>();
    }

    public bool IsComponentActive<T>(int id)
    {
        return !_liveComponents.ContainsKey(typeof(T)) && _liveComponents[typeof(T)].Contains(id);
    }

    public void ActivateComponent<T>(int id)
    {
        if (!_liveComponents.ContainsKey(typeof(T)))
        {
            _liveComponents.Add(typeof(T),new HashSet<int>());
        }

        _liveComponents[typeof(T)].Add(id);
    }

    public void ActivateComponent(Type type, int id)
    {
        _liveComponents[type].Add(id);
    }

    public void DeactivateComponent<T>(int id)
    {
        _liveComponents[typeof(T)].Remove(id);
    }

    public void DetachAll<T>()
    {
        _liveComponents[typeof(T)].Clear();
    }
}