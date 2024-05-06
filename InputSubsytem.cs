public class InputSubSystem
{
    private List<SubscriberData> _Subscribers = new List<SubscriberData>();
    private static InputSubSystem? _Instance = null;
    private bool _active = false;

    public static InputSubSystem GetInputSubSystem()
    {
        if (_Instance == null)
        {
            _Instance = new InputSubSystem();
        }
        return _Instance;
    }

    public void Start()
    {
        if (!_active)
        {
            _active = true;
            Task.Run(() =>
            {
                Listen();
            });
        }
    }

    private void Listen()
    {
        while (_active)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();

                Send(key);
            }
            Task.Delay(2).Wait();
        }
    }

    private void Send(ConsoleKeyInfo info)
    {
        bool isCommand = (info.Modifiers & ConsoleModifiers.Control) != 0;
        bool isUpper = (info.Modifiers & ConsoleModifiers.Shift) != 0;
        
        var subscribers = _Subscribers
            .Where(x => x.CharKey == info.Key && x.IsCommand == isCommand || x.CharKey == ConsoleKey.None)
            .Select(x=>x.Action)
            .ToArray();

        EventData data = new EventData
        {
            IsSpecial = isCommand,
            IsUpper = isUpper,
            Key = info.Key,
        };

        for(int i = 0; i < subscribers.Count(); i++)
        {
            subscribers[i].Invoke(data);
        }
    }

    public void Subscribe(SubscriberData data)
    {
        _Subscribers.Add(data);
    }

    public void Unsubscribe(SubscriberData data)
    {
        _Subscribers.Remove(data);
    }
    
    public class SubscriberData
    {
        public ConsoleKey CharKey { get; set; }
        public bool IsCommand { get; set; }
        public Action<EventData> Action{ get; set; }

        public SubscriberData(ConsoleKey key, Action<EventData> action, bool isCommand)
        {
            CharKey = key;
            IsCommand = isCommand;
            Action = action;
        }
    }

    public class EventData
    {
        public ConsoleKey Key { get; set; }
        public bool IsUpper { get; set; }
        public bool IsSpecial { get; set; }
    }
}