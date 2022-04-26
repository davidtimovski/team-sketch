using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TeamSketch.UserControls;

public partial class ToolsPanel : UserControl
{
    public ToolsPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
