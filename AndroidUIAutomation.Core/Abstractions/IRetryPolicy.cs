using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IRetryPolicy
{
    Task ExecuteAsync(Func<Task> operation, RetryOptions retryOptions, CancellationToken cancellationToken);
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, RetryOptions retryOptions, CancellationToken cancellationToken);
}
