using AndroidUIAutomation.Domain.Configuration;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IAutomationRunner
{
    Task RunAsync(AutomationSequenceConfiguration configuration, int iterations, CancellationToken cancellationToken);
}
