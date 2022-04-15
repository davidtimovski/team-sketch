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

    private async Task CreateButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

        var success = await vm.CreateRoomAsync();
        if (!success)
        {
            return;
        }

        var mainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel()
        };
        mainWindow.Show();

        Close();
    }

    private async Task JoinButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;
        var success = await vm.JoinRoomAsync();
        if (!success)
        {
            return;
        }

        var mainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel()
        };
        mainWindow.Show();

        Close();
    }
}
