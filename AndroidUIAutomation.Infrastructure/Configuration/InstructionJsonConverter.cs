using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Instructions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AndroidUIAutomation.Infrastructure.Configuration;

public sealed class InstructionJsonConverter : JsonConverter<AutomationInstruction>
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, AutomationInstruction? value, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }

    public override AutomationInstruction? ReadJson(JsonReader reader, Type objectType, AutomationInstruction? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var typeText = token["type"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(typeText))
        {
            throw new JsonSerializationException("Instruction 'type' is required.");
        }

        var parsedType = Enum.Parse<InstructionType>(typeText, ignoreCase: true);
        var plainSerializer = JsonSerializer.CreateDefault();
        return parsedType switch
        {
            InstructionType.Find => token.ToObject<FindInstruction>(plainSerializer),
            InstructionType.Press => token.ToObject<PressInstruction>(plainSerializer),
            InstructionType.Wait => token.ToObject<WaitInstruction>(plainSerializer),
            _ => throw new JsonSerializationException($"Unsupported instruction type '{typeText}'.")
        };
    }
}
