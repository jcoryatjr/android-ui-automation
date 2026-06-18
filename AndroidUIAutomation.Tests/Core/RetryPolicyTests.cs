using AndroidUIAutomation.Core.Execution;
using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Tests.TestDoubles;

namespace AndroidUIAutomation.Tests.Core;

[TestClass]
public class RetryPolicyTests
{
    [TestMethod]
    public async Task ExecuteAsync_Retries_WithExponentialBackoff()
    {
        var clock = new FakeClock();
        var policy = new RetryPolicy(clock);
        var attempts = 0;

        var result = await policy.ExecuteAsync(async () =>
        {
            attempts++;
            if (attempts < 3)
            {
                throw new InvalidOperationException("Transient failure");
            }

            await Task.Yield();
            return 42;
        }, new RetryOptions { MaxAttempts = 3, InitialDelayMilliseconds = 100, BackoffMultiplier = 2 }, CancellationToken.None);

        Assert.AreEqual(42, result);
        Assert.AreEqual(2, clock.Delays.Count);
        Assert.AreEqual(TimeSpan.FromMilliseconds(100), clock.Delays[0]);
        Assert.AreEqual(TimeSpan.FromMilliseconds(200), clock.Delays[1]);
    }
}
