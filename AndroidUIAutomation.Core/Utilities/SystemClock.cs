using AndroidUIAutomation.Core.Abstractions;

namespace AndroidUIAutomation.Core.Utilities;

public sealed class SystemClock : IClock
{
    public Task DelayAsync(TimeSpan timeSpan, CancellationToken cancellationToken) => Task.Delay(timeSpan, cancellationToken);
}
