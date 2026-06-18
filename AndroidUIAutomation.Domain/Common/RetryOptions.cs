namespace AndroidUIAutomation.Domain.Common;

public sealed record RetryOptions
{
    public int MaxAttempts { get; init; } = 1;
    public int InitialDelayMilliseconds { get; init; } = 250;
    public double BackoffMultiplier { get; init; } = 2.0;
}
