using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Configuration;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Domain.Instructions;
using Microsoft.Extensions.Logging;

namespace AndroidUIAutomation.Core.Execution;

public sealed class AutomationRunner : IAutomationRunner
{
    private readonly InstructionDispatcher _dispatcher;
    private readonly ILogger<AutomationRunner> _logger;
    private readonly ExecutionContext _context;

    public AutomationRunner(InstructionDispatcher dispatcher, ExecutionContext context, ILogger<AutomationRunner> logger)
    {
        _dispatcher = dispatcher;
        _context = context;
        _logger = logger;
    }

    public async Task RunAsync(AutomationSequenceConfiguration configuration, int iterations, CancellationToken cancellationToken)
    {
        if (!await _context.DeviceClient.IsDeviceConnectedAsync(cancellationToken))
        {
            throw new InvalidOperationException("No Android device is connected through ADB.");
        }

        var totalIterations = Math.Max(1, iterations);
        for (var i = 0; i < totalIterations; i++)
        {
            _logger.LogInformation("Starting sequence {SequenceName} iteration {Iteration}/{TotalIterations}.", configuration.Name, i + 1, totalIterations);
            await RunInstructionListAsync(configuration.Instructions, cancellationToken);

            if (i < totalIterations - 1 && configuration.RandomPauseBetweenIterations is not null)
            {
                var upperExclusive = configuration.RandomPauseBetweenIterations.ValidateAndGetUpperExclusive();
                var pauseMinutes = _context.RandomProvider.Next(configuration.RandomPauseBetweenIterations.LowerBoundMinutes, upperExclusive);
                _logger.LogInformation("Waiting {PauseMinutes} minute(s) before next iteration.", pauseMinutes);
                await _context.Clock.DelayAsync(TimeSpan.FromMinutes(pauseMinutes), cancellationToken);
            }
        }
    }

    private async Task RunInstructionListAsync(IReadOnlyList<AutomationInstruction> instructions, CancellationToken cancellationToken)
    {
        for (var i = 0; i < instructions.Count; i++)
        {
            if (instructions[i] is FindInstruction { RepeatUntilNotFound: true } repeatFind)
            {
                await ExecuteRepeatUntilNotFoundAsync(repeatFind, instructions.Skip(i + 1).ToList(), cancellationToken);
                break;
            }

            await _dispatcher.DispatchAsync(instructions[i], _context, cancellationToken);
        }
    }

    private async Task ExecuteRepeatUntilNotFoundAsync(FindInstruction repeatFind, IReadOnlyList<AutomationInstruction> followUpInstructions, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await _dispatcher.DispatchAsync(repeatFind, _context, cancellationToken);
            if (result is null || !result.Found)
            {
                return;
            }

            foreach (var followUpInstruction in followUpInstructions)
            {
                await _dispatcher.DispatchAsync(followUpInstruction, _context, cancellationToken);
            }
        }
    }
}
