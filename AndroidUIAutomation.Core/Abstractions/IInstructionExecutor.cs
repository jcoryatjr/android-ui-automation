using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IInstructionExecutor
{
    bool CanExecute(AutomationInstruction instruction);
    Task<ImageSearchResult?> ExecuteAsync(AutomationInstruction instruction, global::AndroidUIAutomation.Core.Execution.ExecutionContext context, CancellationToken cancellationToken);
}
