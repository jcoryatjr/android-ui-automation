using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Core.Execution;
using AndroidUIAutomation.Core.Execution.Instructions;
using AndroidUIAutomation.Domain.Configuration;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;
using AndroidUIAutomation.Tests.TestDoubles;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AndroidUIAutomation.Tests.Core;

[TestClass]
public class AutomationRunnerTests
{
    [TestMethod]
    public async Task RunAsync_RepeatsFindSectionUntilNotFound()
    {
        var device = new Mock<IDeviceClient>();
        device.Setup(x => x.IsDeviceConnectedAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var layout = new Mock<IFileSystemLayout>();
        var matcher = new Mock<IImageMatcher>();
        matcher.SetupSequence(x => x.FindImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ImageSearchResult(true, 1, 1, 0.9))
            .ReturnsAsync(new ImageSearchResult(false));

        layout.SetupGet(x => x.ScreenshotsPath).Returns(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        layout.SetupGet(x => x.SearchImagesPath).Returns(Path.GetTempPath());
        Directory.CreateDirectory(layout.Object.ScreenshotsPath);
        await File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), "icon.png"), "x");

        device.Setup(x => x.CaptureScreenshotAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, _) => File.WriteAllTextAsync(path, "x"));

        var strategy = new Mock<IScrollStrategy>();
        strategy.Setup(x => x.BuildPattern(It.IsAny<ScreenSize>(), It.IsAny<int>())).Returns([]);
        var strategyFactory = new Mock<IScrollStrategyFactory>();
        strategyFactory.Setup(x => x.Get(It.IsAny<Domain.Common.ScrollPatternType>())).Returns(strategy.Object);

        var clock = new FakeClock();
        var context = new global::AndroidUIAutomation.Core.Execution.ExecutionContext(
            device.Object,
            matcher.Object,
            layout.Object,
            new RetryPolicy(clock),
            clock,
            new FakeRandomProvider(),
            strategyFactory.Object,
            new ScreenSize(100, 100),
            _ => { });

        var dispatcher = new InstructionDispatcher(new IInstructionExecutor[]
        {
            new FindInstructionExecutor(),
            new PressInstructionExecutor(),
            new WaitInstructionExecutor()
        });

        var runner = new AutomationRunner(dispatcher, context, NullLogger<AutomationRunner>.Instance);
        var config = new AutomationSequenceConfiguration
        {
            Name = "repeat",
            Instructions =
            [
                new FindInstruction { ImageName = "icon.png", RepeatUntilNotFound = true },
                new PressInstruction { X = 1, Y = 1 }
            ]
        };

        await runner.RunAsync(config, 1, CancellationToken.None);

        device.Verify(x => x.TapAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
