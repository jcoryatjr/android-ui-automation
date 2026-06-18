using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Core.Abstractions;

public interface IImageMatcher
{
    Task<ImageSearchResult> FindImageAsync(string screenshotPath, string searchImagePath, CancellationToken cancellationToken);
}
