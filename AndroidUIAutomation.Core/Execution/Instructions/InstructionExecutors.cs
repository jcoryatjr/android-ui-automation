using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;

namespace AndroidUIAutomation.Core.Execution.Instructions;

public sealed class FindInstructionExecutor : IInstructionExecutor
{
    public bool CanExecute(AutomationInstruction instruction) => instruction is FindInstruction;

    public async Task<ImageSearchResult?> ExecuteAsync(AutomationInstruction instruction, global::AndroidUIAutomation.Core.Execution.ExecutionContext context, CancellationToken cancellationToken)
    {
        var findInstruction = (FindInstruction)instruction;
        var imagePath = Path.Combine(context.FileSystemLayout.SearchImagesPath, findInstruction.ImageName);
        var screenshotPath = Path.Combine(context.FileSystemLayout.ScreenshotsPath, $"{DateTime.UtcNow:yyyy-MM-dd_HHmmss_fff}_find.png");

        return await context.RetryPolicy.ExecuteAsync(async () =>
        {
            await context.DeviceClient.CaptureScreenshotAsync(screenshotPath, cancellationToken);
            var result = await context.ImageMatcher.FindImageAsync(screenshotPath, imagePath, cancellationToken);
            if (result.Found)
            {
                context.Log($"Image '{findInstruction.ImageName}' found at ({result.X}, {result.Y}) with confidence {result.Confidence:0.00}.");
                return result;
            }

            foreach (var gesture in context.ScrollStrategyFactory.Get(findInstruction.ScrollPattern).BuildPattern(context.ScreenSize, findInstruction.MaxScrollAttempts))
            {
                await context.DeviceClient.SwipeAsync(gesture, cancellationToken);
                await context.DeviceClient.CaptureScreenshotAsync(screenshotPath, cancellationToken);
                result = await context.ImageMatcher.FindImageAsync(screenshotPath, imagePath, cancellationToken);
                if (result.Found)
                {
                    context.Log($"Image '{findInstruction.ImageName}' found after scroll.");
                    return result;
                }
            }

            context.Log($"Image '{findInstruction.ImageName}' not found.");
            return result;
        }, findInstruction.Retry, cancellationToken);
    }
}

public sealed class PressInstructionExecutor : IInstructionExecutor
{
    public bool CanExecute(AutomationInstruction instruction) => instruction is PressInstruction;

    public async Task<ImageSearchResult?> ExecuteAsync(AutomationInstruction instruction, global::AndroidUIAutomation.Core.Execution.ExecutionContext context, CancellationToken cancellationToken)
    {
        var pressInstruction = (PressInstruction)instruction;
        await context.RetryPolicy.ExecuteAsync(async () =>
        {
            if (pressInstruction.Action == InputActionType.Swipe)
            {
                var swipe = new SwipeGesture(new Point(pressInstruction.X, pressInstruction.Y), new Point(pressInstruction.EndX, pressInstruction.EndY), pressInstruction.DurationMilliseconds);
                await context.DeviceClient.SwipeAsync(swipe, cancellationToken);
                context.Log($"Swipe executed from ({pressInstruction.X}, {pressInstruction.Y}) to ({pressInstruction.EndX}, {pressInstruction.EndY}).");
                return;
            }

            await context.DeviceClient.TapAsync(pressInstruction.X, pressInstruction.Y, cancellationToken);
            context.Log($"Tap executed at ({pressInstruction.X}, {pressInstruction.Y}).");
        }, pressInstruction.Retry, cancellationToken);

        return null;
    }
}

public sealed class WaitInstructionExecutor : IInstructionExecutor
{
    public bool CanExecute(AutomationInstruction instruction) => instruction is WaitInstruction;

    public async Task<ImageSearchResult?> ExecuteAsync(AutomationInstruction instruction, global::AndroidUIAutomation.Core.Execution.ExecutionContext context, CancellationToken cancellationToken)
    {
        var waitInstruction = (WaitInstruction)instruction;
        if (waitInstruction.LowerBoundMinutes < 0 || waitInstruction.UpperBoundMinutes < waitInstruction.LowerBoundMinutes)
        {
            throw new InvalidOperationException("Wait instruction contains invalid bounds.");
        }

        var minutes = context.RandomProvider.Next(waitInstruction.LowerBoundMinutes, waitInstruction.UpperBoundMinutes + 1);
        context.Log($"Waiting for {minutes} minute(s).");
        await context.Clock.DelayAsync(TimeSpan.FromMinutes(minutes), cancellationToken);
        return null;
    }
}
