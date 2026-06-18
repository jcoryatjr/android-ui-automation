using System.Windows;

namespace AndroidUIAutomation.UI.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Execution starts from the shared core services.", "Start");
    }

    private void PauseButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Pause requested.", "Pause");
    }

    private void ResumeButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Resume requested.", "Resume");
    }

    private void StopButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Stop requested.", "Stop");
    }
}
