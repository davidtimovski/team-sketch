using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TeamSketch.UserControls;

public partial class ConnectionStatus : UserControl
{
    public ConnectionStatus()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
