using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TeamSketch.UserControls;

public partial class ParticipantsPanel : UserControl
{
    public ParticipantsPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
