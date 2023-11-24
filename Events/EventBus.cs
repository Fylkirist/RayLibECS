namespace RayLibECS.Events;

public class EventBus
{
    private static EventBus? _instance;
    public static EventBus Instance => _instance ??= new EventBus();
    
    public delegate void EventHandler<T>(T eventData);

    private readonly Dictionary<Type, object> _eventSubscriptions = new Dictionary<Type, object>();
    
    public void Subscribe<T>(EventHandler<T> handler)
    {
        Type eventType = typeof(T);

        if (!_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType] = new List<Delegate>();
        }

        var handlers = (List<Delegate>)_eventSubscriptions[eventType];
        handlers.Add(handler);
    }

    public void Unsubscribe<T>(EventHandler<T> handler)
    {
        Type eventType = typeof(T);

        if (_eventSubscriptions.ContainsKey(eventType))
        {
            var handlers = (List<Delegate>)_eventSubscriptions[eventType];
            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                _eventSubscriptions.Remove(eventType);
            }
        }
    }

    public void Publish<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (_eventSubscriptions.ContainsKey(eventType))
        {
            var handlers = (List<Delegate>)_eventSubscriptions[eventType];

            foreach (var handler in handlers)
            {
                ((EventHandler<T>)handler)?.Invoke(eventData);
            }
        }
    }
}

