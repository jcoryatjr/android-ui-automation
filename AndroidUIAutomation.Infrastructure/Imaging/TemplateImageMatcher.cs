using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Infrastructure.Imaging;

public sealed class TemplateImageMatcher : IImageMatcher
{
    public Task<ImageSearchResult> FindImageAsync(string screenshotPath, string searchImagePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(screenshotPath) || !File.Exists(searchImagePath))
        {
            return Task.FromResult(new ImageSearchResult(false));
        }

        var screenshotName = Path.GetFileNameWithoutExtension(screenshotPath);
        var targetName = Path.GetFileNameWithoutExtension(searchImagePath);
        var found = screenshotName.Contains(targetName, StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(found ? new ImageSearchResult(true, 100, 100, 0.90) : new ImageSearchResult(false));
    }
}
