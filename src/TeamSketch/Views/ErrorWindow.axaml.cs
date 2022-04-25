using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TeamSketch.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
