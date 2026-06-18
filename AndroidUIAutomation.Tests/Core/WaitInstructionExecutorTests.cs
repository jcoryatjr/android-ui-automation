using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Core.Execution;
using AndroidUIAutomation.Core.Execution.Instructions;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;
using AndroidUIAutomation.Tests.TestDoubles;
using Moq;

namespace AndroidUIAutomation.Tests.Core;

[TestClass]
public class WaitInstructionExecutorTests
{
    [TestMethod]
    public async Task ExecuteAsync_UsesConfiguredRandomBounds()
    {
        var clock = new FakeClock();
        var context = new global::AndroidUIAutomation.Core.Execution.ExecutionContext(
            new Mock<IDeviceClient>().Object,
            new Mock<IImageMatcher>().Object,
            new Mock<IFileSystemLayout>().Object,
            new RetryPolicy(clock),
            clock,
            new FakeRandomProvider(2),
            new Mock<IScrollStrategyFactory>().Object,
            new ScreenSize(1000, 1000),
            _ => { });

        var executor = new WaitInstructionExecutor();
        await executor.ExecuteAsync(new WaitInstruction { LowerBoundMinutes = 1, UpperBoundMinutes = 3 }, context, CancellationToken.None);

        Assert.AreEqual(1, clock.Delays.Count);
        Assert.AreEqual(TimeSpan.FromMinutes(2), clock.Delays[0]);
    }
}
