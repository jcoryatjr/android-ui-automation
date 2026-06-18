using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IScrollStrategy
{
    ScrollPatternType Pattern { get; }
    IReadOnlyList<SwipeGesture> BuildPattern(ScreenSize screenSize, int maxScrollAttempts);
}
