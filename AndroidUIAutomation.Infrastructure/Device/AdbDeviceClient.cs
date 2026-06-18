using System.Diagnostics;
using AndroidUIAutomation.Core.Abstractions;
using AndroidUIAutomation.Domain.Execution;

namespace AndroidUIAutomation.Infrastructure.Device;

public sealed class AdbDeviceClient : IDeviceClient
{
    private readonly string _adbExecutable;

    public AdbDeviceClient(string adbExecutable = "adb")
    {
        _adbExecutable = adbExecutable;
    }

    public async Task<bool> IsDeviceConnectedAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteAsync("devices", cancellationToken);
        return output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Any(line => line.EndsWith("\tdevice", StringComparison.OrdinalIgnoreCase));
    }

    public Task CaptureScreenshotAsync(string outputPath, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var command = $"exec-out screencap -p > \"{outputPath}\"";
        return ExecuteShellPipelineAsync(command, cancellationToken);
    }

    public Task TapAsync(int x, int y, CancellationToken cancellationToken) =>
        ExecuteAsync($"shell input tap {x} {y}", cancellationToken);

    public Task SwipeAsync(SwipeGesture gesture, CancellationToken cancellationToken) =>
        ExecuteAsync($"shell input swipe {gesture.Start.X} {gesture.Start.Y} {gesture.End.X} {gesture.End.Y} {gesture.DurationMilliseconds}", cancellationToken);

    private async Task<string> ExecuteAsync(string arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _adbExecutable,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start adb process.");
        var stdOut = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"adb command failed ({arguments}): {stdErr}");
        }

        return stdOut;
    }

    private async Task ExecuteShellPipelineAsync(string command, CancellationToken cancellationToken)
    {
        var shell = OperatingSystem.IsWindows() ? "cmd" : "/bin/bash";
        var shellArgs = OperatingSystem.IsWindows()
            ? $"/c \"{_adbExecutable} {command}\""
            : $"-lc \"{_adbExecutable} {command}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = shell,
            Arguments = shellArgs,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start adb shell process.");
        var stdErr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"adb shell command failed ({command}): {stdErr}");
        }
    }
}
