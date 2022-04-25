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

        var result = await vm.CreateRoomAsync();
        if (result.Success)
        {
            Start();
        }
        else if (result.ShowError)
        {
            ShowError(result.ErrorMessage, result.IsSystemError);
        }
    }

    private async Task JoinButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

        var result = await vm.JoinRoomAsync();
        if (result.Success)
        {
            Start();
        }
        else if (result.ShowError)
        {
            ShowError(result.ErrorMessage, result.IsSystemError);
        }
    }

    private void ShowError(string message, bool isSystemError)
    {
        var mainWindow = new ErrorWindow
        {
            DataContext = new ErrorViewModel(message, isSystemError),
            Topmost = true,
            CanResize = false
        };
        mainWindow.Show();
    }

    private void Start()
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
        mainWindow.Show();

        Close();
    }
}
