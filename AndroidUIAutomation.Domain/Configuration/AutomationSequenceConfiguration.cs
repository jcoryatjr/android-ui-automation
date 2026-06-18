using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Instructions;

namespace AndroidUIAutomation.Domain.Configuration;

public sealed record AutomationSequenceConfiguration
{
    public string Name { get; init; } = string.Empty;
    public RandomRange? RandomPauseBetweenIterations { get; init; }
    public IReadOnlyList<AutomationInstruction> Instructions { get; init; } = [];
}
