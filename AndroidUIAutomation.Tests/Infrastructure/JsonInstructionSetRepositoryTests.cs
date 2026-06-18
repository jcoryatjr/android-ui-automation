using AndroidUIAutomation.Infrastructure.Configuration;

namespace AndroidUIAutomation.Tests.Infrastructure;

[TestClass]
public class JsonInstructionSetRepositoryTests
{
    [TestMethod]
    public async Task LoadAsync_ParsesMixedInstructionTypes()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(tempFile, """
        {
          "name": "TestSequence",
          "randomPauseBetweenIterations": { "lowerBoundMinutes": 1, "upperBoundMinutes": 2 },
          "instructions": [
            { "type": "find", "imageName": "button.png" },
            { "type": "press", "x": 100, "y": 200 },
            { "type": "wait", "lowerBoundMinutes": 0, "upperBoundMinutes": 1 }
          ]
        }
        """);

        var repository = new JsonInstructionSetRepository();
        var result = await repository.LoadAsync(tempFile, CancellationToken.None);

        Assert.AreEqual("TestSequence", result.Name);
        Assert.AreEqual(3, result.Instructions.Count);
        Assert.AreEqual(Domain.Common.InstructionType.Find, result.Instructions[0].Type);
        Assert.AreEqual(Domain.Common.InstructionType.Press, result.Instructions[1].Type);
        Assert.AreEqual(Domain.Common.InstructionType.Wait, result.Instructions[2].Type);
    }
}
