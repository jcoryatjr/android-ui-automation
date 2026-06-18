namespace AndroidUIAutomation.Domain.Execution;

public readonly record struct ScreenSize(int Width, int Height);

public readonly record struct Point(int X, int Y);

public readonly record struct SwipeGesture(Point Start, Point End, int DurationMilliseconds = 250);
