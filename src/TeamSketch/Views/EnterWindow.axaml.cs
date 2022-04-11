using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using TeamSketch.ViewModels;

namespace TeamSketch.Views;

public partial class EnterWindow : Window
{
    public EnterWindow()
    {
        InitializeComponent();

        var connectButton = this.FindControl<Button>("connectButton");
        connectButton.Command = ReactiveCommand.Create(ConnectButtonClicked);
    }

    public event EventHandler<EventArgs> Entered;

    private async Task ConnectButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

        var result = await vm.ConnectAsync();
        if (result != null)
        {
            // TODO: error message
            return;
        }

        Entered.Invoke(this, null);
        Close();
    }
}
