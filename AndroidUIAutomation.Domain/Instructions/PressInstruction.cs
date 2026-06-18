using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Domain.Instructions;

public sealed record PressInstruction : AutomationInstruction
{
    public override InstructionType Type => InstructionType.Press;
    public InputActionType Action { get; init; } = InputActionType.Tap;
    public int X { get; init; }
    public int Y { get; init; }
    public int EndX { get; init; }
    public int EndY { get; init; }
    public int DurationMilliseconds { get; init; } = 200;
    public RetryOptions Retry { get; init; } = new();
}
