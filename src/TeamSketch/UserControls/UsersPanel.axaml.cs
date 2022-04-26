using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TeamSketch.UserControls;

public partial class UsersPanel : UserControl
{
    public UsersPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
