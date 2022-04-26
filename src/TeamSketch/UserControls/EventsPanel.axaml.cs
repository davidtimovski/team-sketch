using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TeamSketch.UserControls;

public partial class EventsPanel : UserControl
{
    public EventsPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
