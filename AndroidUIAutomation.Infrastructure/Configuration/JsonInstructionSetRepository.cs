using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AndroidUIAutomation.Infrastructure.Configuration;

public sealed class JsonInstructionSetRepository : IInstructionSetRepository
{
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Converters = [new InstructionJsonConverter()]
    };

    public async Task<AutomationSequenceConfiguration> LoadAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Instruction set file not found: {filePath}", filePath);
        }

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var config = JsonConvert.DeserializeObject<AutomationSequenceConfiguration>(json, _serializerSettings);
        if (config is null)
        {
            throw new InvalidOperationException($"Instruction set file '{filePath}' could not be deserialized.");
        }

        return config;
    }
}
