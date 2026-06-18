namespace AndroidUIAutomation.Core.Abstractions;

public interface IClock
{
    Task DelayAsync(TimeSpan timeSpan, CancellationToken cancellationToken);
}
