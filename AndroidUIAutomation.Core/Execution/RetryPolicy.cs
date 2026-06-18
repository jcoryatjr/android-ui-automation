using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Core.Execution;

public sealed class RetryPolicy : IRetryPolicy
{
    private readonly IClock _clock;

    public RetryPolicy(IClock clock)
    {
        _clock = clock;
    }

    public Task ExecuteAsync(Func<Task> operation, RetryOptions retryOptions, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            await operation();
            return true;
        }, retryOptions, cancellationToken);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, RetryOptions retryOptions, CancellationToken cancellationToken)
    {
        var options = retryOptions with { MaxAttempts = Math.Max(1, retryOptions.MaxAttempts) };
        var delayMs = Math.Max(1, options.InitialDelayMilliseconds);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= options.MaxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await operation();
            }
            catch (Exception ex) when (attempt < options.MaxAttempts)
            {
                lastException = ex;
                await _clock.DelayAsync(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
                delayMs = (int)Math.Ceiling(delayMs * Math.Max(1.0, options.BackoffMultiplier));
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }
        }

        throw lastException ?? new InvalidOperationException("Retry policy failed without exception details.");
    }
}
