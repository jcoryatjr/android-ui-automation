using AndroidUIAutomation.Domain.Configuration;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IInstructionSetRepository
{
    Task<AutomationSequenceConfiguration> LoadAsync(string filePath, CancellationToken cancellationToken);
}
