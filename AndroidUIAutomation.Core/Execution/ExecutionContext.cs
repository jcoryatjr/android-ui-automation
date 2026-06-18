using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Core.Execution;

public sealed record ExecutionContext(
    IDeviceClient DeviceClient,
    IImageMatcher ImageMatcher,
    IFileSystemLayout FileSystemLayout,
    IRetryPolicy RetryPolicy,
    IClock Clock,
    IRandomProvider RandomProvider,
    IScrollStrategyFactory ScrollStrategyFactory,
    ScreenSize ScreenSize,
    Action<string> Log);
