using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Domain.Instructions;

public abstract record AutomationInstruction
{
    public abstract InstructionType Type { get; }
}
