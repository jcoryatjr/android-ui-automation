using AndroidUIAutomation.Core.Abstractions;

namespace AndroidUIAutomation.Core.Utilities;

public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
}
