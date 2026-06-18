using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IDeviceClient
{
    Task<bool> IsDeviceConnectedAsync(CancellationToken cancellationToken);
    Task CaptureScreenshotAsync(string outputPath, CancellationToken cancellationToken);
    Task TapAsync(int x, int y, CancellationToken cancellationToken);
    Task SwipeAsync(SwipeGesture gesture, CancellationToken cancellationToken);
}
