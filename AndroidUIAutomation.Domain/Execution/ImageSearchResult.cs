namespace AndroidUIAutomation.Domain.Execution;

public sealed record ImageSearchResult(bool Found, int X = 0, int Y = 0, double Confidence = 0);
