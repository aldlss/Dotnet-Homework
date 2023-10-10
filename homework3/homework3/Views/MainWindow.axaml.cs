using Avalonia.Controls;

namespace homework3.Views;

public partial class MainWindow : Window
{
    public static Window Instance { get; private set; }
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
    }
}
