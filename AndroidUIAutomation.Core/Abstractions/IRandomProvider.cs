namespace AndroidUIAutomation.Core.Abstractions;

public interface IRandomProvider
{
    int Next(int minInclusive, int maxExclusive);
}
