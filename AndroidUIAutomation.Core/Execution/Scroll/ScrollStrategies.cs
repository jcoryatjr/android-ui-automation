using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Core.Execution.Scroll;

public sealed class ZigZagScrollStrategy : IScrollStrategy
{
    public ScrollPatternType Pattern => ScrollPatternType.ZigZag;

    public IReadOnlyList<SwipeGesture> BuildPattern(ScreenSize screenSize, int maxScrollAttempts)
    {
        var gestures = new List<SwipeGesture>();
        var attempts = Math.Max(0, maxScrollAttempts);
        for (var i = 0; i < attempts; i++)
        {
            var leftToRight = i % 2 == 0;
            var startX = leftToRight ? screenSize.Width / 4 : (screenSize.Width * 3) / 4;
            var endX = leftToRight ? (screenSize.Width * 3) / 4 : screenSize.Width / 4;
            gestures.Add(new SwipeGesture(new Point(startX, (screenSize.Height * 3) / 4), new Point(endX, screenSize.Height / 4)));
        }

        return gestures;
    }
}

public sealed class SpiralScrollStrategy : IScrollStrategy
{
    public ScrollPatternType Pattern => ScrollPatternType.Spiral;

    public IReadOnlyList<SwipeGesture> BuildPattern(ScreenSize screenSize, int maxScrollAttempts)
    {
        var gestures = new List<SwipeGesture>();
        var attempts = Math.Max(0, maxScrollAttempts);
        var center = new Point(screenSize.Width / 2, screenSize.Height / 2);
        for (var i = 0; i < attempts; i++)
        {
            var radius = Math.Min(screenSize.Width, screenSize.Height) / (4 + i);
            var end = i % 2 == 0
                ? new Point(Math.Max(0, center.X - radius), Math.Max(0, center.Y - radius))
                : new Point(Math.Min(screenSize.Width - 1, center.X + radius), Math.Min(screenSize.Height - 1, center.Y + radius));

            gestures.Add(new SwipeGesture(center, end));
        }

        return gestures;
    }
}

public sealed class GridScrollStrategy : IScrollStrategy
{
    public ScrollPatternType Pattern => ScrollPatternType.Grid;

    public IReadOnlyList<SwipeGesture> BuildPattern(ScreenSize screenSize, int maxScrollAttempts)
    {
        var gestures = new List<SwipeGesture>();
        var attempts = Math.Max(0, maxScrollAttempts);
        var columns = 3;
        for (var i = 0; i < attempts; i++)
        {
            var column = i % columns;
            var x = (screenSize.Width * (column + 1)) / (columns + 1);
            gestures.Add(new SwipeGesture(new Point(x, (screenSize.Height * 4) / 5), new Point(x, screenSize.Height / 5)));
        }

        return gestures;
    }
}

public sealed class RandomScrollStrategy : IScrollStrategy
{
    private readonly IRandomProvider _randomProvider;

    public RandomScrollStrategy(IRandomProvider randomProvider)
    {
        _randomProvider = randomProvider;
    }

    public ScrollPatternType Pattern => ScrollPatternType.Random;

    public IReadOnlyList<SwipeGesture> BuildPattern(ScreenSize screenSize, int maxScrollAttempts)
    {
        var gestures = new List<SwipeGesture>();
        for (var i = 0; i < Math.Max(0, maxScrollAttempts); i++)
        {
            var start = new Point(_randomProvider.Next(0, Math.Max(1, screenSize.Width)), _randomProvider.Next(0, Math.Max(1, screenSize.Height)));
            var end = new Point(_randomProvider.Next(0, Math.Max(1, screenSize.Width)), _randomProvider.Next(0, Math.Max(1, screenSize.Height)));
            gestures.Add(new SwipeGesture(start, end));
        }

        return gestures;
    }
}

public sealed class ScrollStrategyFactory : IScrollStrategyFactory
{
    private readonly IReadOnlyDictionary<ScrollPatternType, IScrollStrategy> _strategies;

    public ScrollStrategyFactory(IEnumerable<IScrollStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Pattern);
    }

    public IScrollStrategy Get(ScrollPatternType patternType)
    {
        if (_strategies.TryGetValue(patternType, out var strategy))
        {
            return strategy;
        }

        throw new InvalidOperationException($"No scroll strategy registered for {patternType}.");
    }
}
