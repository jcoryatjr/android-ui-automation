using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;

namespace AndroidUIAutomation.Core.Execution;

public sealed class InstructionDispatcher
{
    private readonly IReadOnlyList<IInstructionExecutor> _executors;

    public InstructionDispatcher(IEnumerable<IInstructionExecutor> executors)
    {
        _executors = executors.ToList();
    }

    public Task<ImageSearchResult?> DispatchAsync(AutomationInstruction instruction, ExecutionContext context, CancellationToken cancellationToken)
    {
        var executor = _executors.FirstOrDefault(e => e.CanExecute(instruction));
        if (executor is null)
        {
            throw new InvalidOperationException($"No executor registered for instruction type {instruction.Type}.");
        }

        return executor.ExecuteAsync(instruction, context, cancellationToken);
    }
}
