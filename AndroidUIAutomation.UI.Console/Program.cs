using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Core.Execution;
using AndroidUIAutomation.Core.Execution.Instructions;
using AndroidUIAutomation.Core.Execution.Scroll;
using AndroidUIAutomation.Core.Utilities;
using AndroidUIAutomation.Domain.Common;
using AndroidUIAutomation.Domain.Execution;
using AndroidUIAutomation.Infrastructure.Configuration;
using AndroidUIAutomation.Infrastructure.Device;
using AndroidUIAutomation.Infrastructure.Files;
using AndroidUIAutomation.Infrastructure.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var cancellationToken = ConsoleCancelExtensions.CreateCancellationToken();
var options = AppOptions.Parse(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(options.DebugEnabled ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console()
    .CreateLogger();

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddSerilog(Log.Logger, dispose: true));

var rootPath = AppContext.BaseDirectory;
var fileSystemLayout = new AppFileSystemLayout(rootPath);
fileSystemLayout.EnsureFolders();

services.AddSingleton<IFileSystemLayout>(fileSystemLayout);
services.AddSingleton<IInstructionSetRepository, JsonInstructionSetRepository>();
services.AddSingleton<IDeviceClient>(_ => new AdbDeviceClient(options.AdbExecutable));
services.AddSingleton<IImageMatcher, TemplateImageMatcher>();
services.AddSingleton<IClock, SystemClock>();
services.AddSingleton<IRandomProvider, SystemRandomProvider>();
services.AddSingleton<IRetryPolicy, RetryPolicy>();
services.AddSingleton<IScrollStrategy, ZigZagScrollStrategy>();
services.AddSingleton<IScrollStrategy, SpiralScrollStrategy>();
services.AddSingleton<IScrollStrategy, GridScrollStrategy>();
services.AddSingleton<IScrollStrategy, RandomScrollStrategy>();
services.AddSingleton<IScrollStrategyFactory, ScrollStrategyFactory>();
services.AddSingleton<IInstructionExecutor, FindInstructionExecutor>();
services.AddSingleton<IInstructionExecutor, PressInstructionExecutor>();
services.AddSingleton<IInstructionExecutor, WaitInstructionExecutor>();
services.AddSingleton<InstructionDispatcher>();
services.AddSingleton(sp => new global::AndroidUIAutomation.Core.Execution.ExecutionContext(
    sp.GetRequiredService<IDeviceClient>(),
    sp.GetRequiredService<IImageMatcher>(),
    sp.GetRequiredService<IFileSystemLayout>(),
    sp.GetRequiredService<IRetryPolicy>(),
    sp.GetRequiredService<IClock>(),
    sp.GetRequiredService<IRandomProvider>(),
    sp.GetRequiredService<IScrollStrategyFactory>(),
    new ScreenSize(1920, 1200),
    message => sp.GetRequiredService<ILoggerFactory>().CreateLogger("Execution").LogInformation("{Message}", message)));
services.AddSingleton<IAutomationRunner, AutomationRunner>();

using var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");

try
{
    if (options.CaptureScreenshot)
    {
        await CaptureScreenshotAsync(serviceProvider, options, cancellationToken);
        return;
    }

    var instructionPath = ResolveInstructionPath(fileSystemLayout, options.InstructionSetFileName);
    logger.LogInformation("Loading instruction set from {InstructionPath}.", instructionPath);

    var repository = serviceProvider.GetRequiredService<IInstructionSetRepository>();
    var configuration = await repository.LoadAsync(instructionPath, cancellationToken);
    var runner = serviceProvider.GetRequiredService<IAutomationRunner>();

    await runner.RunAsync(configuration, options.Iterations, cancellationToken);
    logger.LogInformation("Automation sequence completed.");
}
catch (OperationCanceledException)
{
    logger.LogWarning("Operation cancelled.");
    Environment.ExitCode = 2;
}
catch (Exception ex)
{
    logger.LogError(ex, "Automation run failed.");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

static async Task CaptureScreenshotAsync(IServiceProvider serviceProvider, AppOptions options, CancellationToken cancellationToken)
{
    var device = serviceProvider.GetRequiredService<IDeviceClient>();
    var fileSystemLayout = serviceProvider.GetRequiredService<IFileSystemLayout>();
    var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Capture");

    if (!await device.IsDeviceConnectedAsync(cancellationToken))
    {
        throw new InvalidOperationException("No Android device is connected through ADB.");
    }

    var name = options.ScreenshotName;
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.Write("Enter screenshot name: ");
        name = Console.ReadLine();
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        throw new InvalidOperationException("A screenshot name is required.");
    }

    var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
    var fileName = $"{DateTime.UtcNow:yyyy-MM-dd_HHmmss}_{safeName}.png";
    var destination = Path.Combine(fileSystemLayout.ScreenshotsPath, fileName);
    await device.CaptureScreenshotAsync(destination, cancellationToken);
    logger.LogInformation("Screenshot saved to {Destination}.", destination);
}

static string ResolveInstructionPath(IFileSystemLayout layout, string fileNameOrPath)
{
    if (Path.IsPathRooted(fileNameOrPath) || fileNameOrPath.Contains(Path.DirectorySeparatorChar) || fileNameOrPath.Contains(Path.AltDirectorySeparatorChar))
    {
        return fileNameOrPath;
    }

    return Path.Combine(layout.InstructionSetsPath, fileNameOrPath);
}

internal sealed record AppOptions
{
    public bool DebugEnabled { get; init; }
    public bool CaptureScreenshot { get; init; }
    public string ScreenshotName { get; init; } = string.Empty;
    public string InstructionSetFileName { get; init; } = "sequence1.json";
    public string AdbExecutable { get; init; } = "adb";
    public int Iterations { get; init; } = 1;

    public static AppOptions Parse(string[] args)
    {
        var options = new AppOptions();
        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];
            switch (current)
            {
                case "--debug":
                    options = options with { DebugEnabled = true };
                    break;
                case "--capture":
                    options = options with { CaptureScreenshot = true };
                    break;
                case "--name":
                    options = options with { ScreenshotName = GetValue(args, ++i, "--name") };
                    break;
                case "--instruction-set":
                    options = options with { InstructionSetFileName = GetValue(args, ++i, "--instruction-set") };
                    break;
                case "--adb":
                    options = options with { AdbExecutable = GetValue(args, ++i, "--adb") };
                    break;
                case "--iterations":
                    options = options with { Iterations = int.Parse(GetValue(args, ++i, "--iterations")) };
                    break;
            }
        }

        return options;
    }

    private static string GetValue(string[] args, int index, string name)
    {
        if (index >= args.Length)
        {
            throw new ArgumentException($"Expected value after {name}.");
        }

        return args[index];
    }
}

internal static class ConsoleCancelExtensions
{
    public static CancellationToken CreateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        return cts.Token;
    }
}
