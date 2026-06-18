using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Domain.Instructions;

public sealed record FindInstruction : AutomationInstruction
{
    public override InstructionType Type => InstructionType.Find;
    public string ImageName { get; init; } = string.Empty;
    public ScrollPatternType ScrollPattern { get; init; } = ScrollPatternType.ZigZag;
    public bool RepeatUntilNotFound { get; init; }
    public int MaxScrollAttempts { get; init; } = 4;
    public RetryOptions Retry { get; init; } = new();
}
