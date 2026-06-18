using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Domain.Instructions;

public sealed record WaitInstruction : AutomationInstruction
{
    public override InstructionType Type => InstructionType.Wait;
    public int LowerBoundMinutes { get; init; }
    public int UpperBoundMinutes { get; init; }
}
