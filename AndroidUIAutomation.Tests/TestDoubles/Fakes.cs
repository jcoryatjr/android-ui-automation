using AndroidUIAutomation.Core.Abstractions;

namespace AndroidUIAutomation.Tests.TestDoubles;

internal sealed class FakeClock : IClock
{
    public List<TimeSpan> Delays { get; } = [];

    public Task DelayAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        Delays.Add(timeSpan);
        return Task.CompletedTask;
    }
}

internal sealed class FakeRandomProvider : IRandomProvider
{
    private readonly Queue<int> _values;

    public FakeRandomProvider(params int[] values)
    {
        _values = new Queue<int>(values);
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        if (_values.Count == 0)
        {
            return minInclusive;
        }

        var value = _values.Dequeue();
        return value < minInclusive ? minInclusive : Math.Min(value, maxExclusive - 1);
    }
}
