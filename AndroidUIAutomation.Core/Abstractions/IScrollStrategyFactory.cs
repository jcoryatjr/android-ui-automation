using AndroidUIAutomation.Domain.Common;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IScrollStrategyFactory
{
    IScrollStrategy Get(ScrollPatternType patternType);
}
