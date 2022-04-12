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

        var createButton = this.FindControl<Button>("createButton");
        createButton.Command = ReactiveCommand.Create(CreateButtonClicked);

        var joinButton = this.FindControl<Button>("joinButton");
        joinButton.Command = ReactiveCommand.Create(JoinButtonClicked);
    }

    public event EventHandler<EventArgs> Entered;

    private async Task CreateButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

        var result = await vm.CreateRoomAsync();
        if (result != null)
        {
            // TODO: error message
            return;
        }

        Entered.Invoke(this, null);
        Close();
    }

    private async Task JoinButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

        var result = await vm.JoinRoomAsync();
        if (result != null)
        {
            // TODO: error message
            return;
        }

        Entered.Invoke(this, null);
        Close();
    }
}
