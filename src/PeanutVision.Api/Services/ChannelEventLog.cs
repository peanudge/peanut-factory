namespace PeanutVision.Api.Services;

public enum ChannelEventType
{
    FrameDropped,
    BufferUnavailable,
    AcquisitionError,
    AcquisitionStarted,
    AcquisitionStopped,
}

public readonly record struct ChannelEvent(
    DateTime Timestamp,
    ChannelEventType Type,
    string Message);

public sealed class ChannelEventLog
{
    private readonly ChannelEvent[] _buffer;
    private int _head;
    private int _count;
    private readonly object _lock = new();

    public ChannelEventLog(int capacity = 100)
    {
        _buffer = new ChannelEvent[capacity];
    }

    public void Add(ChannelEvent evt)
    {
        lock (_lock)
        {
            _buffer[_head] = evt;
            _head = (_head + 1) % _buffer.Length;
            if (_count < _buffer.Length)
                _count++;
        }
    }

    public ChannelEvent[] GetRecent(int max = 50)
    {
        lock (_lock)
        {
            var count = Math.Min(max, _count);
            var result = new ChannelEvent[count];

            for (int i = 0; i < count; i++)
            {
                var index = (_head - 1 - i + _buffer.Length) % _buffer.Length;
                result[i] = _buffer[index];
            }

            return result;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _head = 0;
            _count = 0;
        }
    }
}
