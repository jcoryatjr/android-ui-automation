namespace AndroidUIAutomation.Core.Abstractions;

public interface IFileSystemLayout
{
    string RootPath { get; }
    string InstructionSetsPath { get; }
    string SearchImagesPath { get; }
    string ScreenshotsPath { get; }
    void EnsureFolders();
}
