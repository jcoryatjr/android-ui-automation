using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Core.Execution;
using AndroidUIAutomation.Core.Execution.Instructions;
using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;
using AndroidUIAutomation.Tests.TestDoubles;
using Moq;

namespace AndroidUIAutomation.Tests.Core;

[TestClass]
public class FindInstructionExecutorTests
{
    [TestMethod]
    public async Task ExecuteAsync_ScrollsUntilImageFound()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        try
        {
            var layout = new Mock<IFileSystemLayout>();
            layout.SetupGet(x => x.ScreenshotsPath).Returns(tempRoot);
            layout.SetupGet(x => x.SearchImagesPath).Returns(tempRoot);
            await File.WriteAllTextAsync(Path.Combine(tempRoot, "button.png"), "x");

            var device = new Mock<IDeviceClient>();
            device.Setup(x => x.CaptureScreenshotAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((path, _) => File.WriteAllTextAsync(path, "s"));

            var matcher = new Mock<IImageMatcher>();
            matcher.SetupSequence(x => x.FindImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageSearchResult(false))
                .ReturnsAsync(new ImageSearchResult(true, 50, 60, 0.99));

            var strategy = new Mock<IScrollStrategy>();
            strategy.SetupGet(x => x.Pattern).Returns(ScrollPatternType.ZigZag);
            strategy.Setup(x => x.BuildPattern(It.IsAny<ScreenSize>(), It.IsAny<int>()))
                .Returns([new SwipeGesture(new Point(0, 0), new Point(1, 1))]);

            var strategyFactory = new Mock<IScrollStrategyFactory>();
            strategyFactory.Setup(x => x.Get(ScrollPatternType.ZigZag)).Returns(strategy.Object);

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

            var executor = new FindInstructionExecutor();
            var result = await executor.ExecuteAsync(new FindInstruction { ImageName = "button.png", ScrollPattern = ScrollPatternType.ZigZag }, context, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Found);
            device.Verify(x => x.SwipeAsync(It.IsAny<SwipeGesture>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }
}
