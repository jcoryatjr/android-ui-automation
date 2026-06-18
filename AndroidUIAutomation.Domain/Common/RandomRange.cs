namespace AndroidUIAutomation.Domain.Common;

public sealed record RandomRange
{
    public int LowerBoundMinutes { get; init; }
    public int UpperBoundMinutes { get; init; }

    public int ValidateAndGetUpperExclusive()
    {
        if (LowerBoundMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(LowerBoundMinutes), "Lower bound must be greater than or equal to 0.");
        }

        if (UpperBoundMinutes < LowerBoundMinutes)
        {
            throw new ArgumentOutOfRangeException(nameof(UpperBoundMinutes), "Upper bound must be greater than or equal to lower bound.");
        }

        return UpperBoundMinutes + 1;
    }
}
