using AndroidUIAutomation.Core.Abstractions;

namespace AndroidUIAutomation.Infrastructure.Files;

public sealed class AppFileSystemLayout : IFileSystemLayout
{
    public AppFileSystemLayout(string rootPath)
    {
        RootPath = rootPath;
        InstructionSetsPath = Path.Combine(rootPath, "InstructionSets");
        SearchImagesPath = Path.Combine(rootPath, "SearchImages");
        ScreenshotsPath = Path.Combine(rootPath, "Screenshots");
    }

    public string RootPath { get; }
    public string InstructionSetsPath { get; }
    public string SearchImagesPath { get; }
    public string ScreenshotsPath { get; }

    public void EnsureFolders()
    {
        Directory.CreateDirectory(InstructionSetsPath);
        Directory.CreateDirectory(SearchImagesPath);
        Directory.CreateDirectory(ScreenshotsPath);
    }
}
