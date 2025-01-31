using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.ViewModels;

namespace TeamSketch.Views;

public partial class LobbyWindow : Window
{
    private readonly IAppState _appState;

    public LobbyWindow()
    {
        InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();
        
        CreateButton.Command = ReactiveCommand.Create(CreateButtonClicked);
        JoinButton.Command = ReactiveCommand.Create(JoinButtonClicked);
        JoinRandomButton.Command = ReactiveCommand.Create(JoinRandomButtonClicked);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        var vm = (LobbyViewModel)DataContext;

        vm.SignalRService.Connection.On<string>("RandomRoomJoined", (room) =>
        {
            _appState.Room = room;
            Start(vm.SignalRService);
        });

        base.OnDataContextChanged(e);
    }

    private async Task CreateButtonClicked()
    {
        var vm = (LobbyViewModel)DataContext;

        var result = await vm.CreateRoomAsync();
        if (result.Success)
        {
            Start(vm.SignalRService);
        }
        else if (result.ShowError)
        {
            ShowError(result.ErrorMessage, result.IsSystemError);
        }
    }

    private async Task JoinButtonClicked()
    {
        var vm = (LobbyViewModel)DataContext;

        var result = await vm.JoinRoomAsync();
        if (result.Success)
        {
            Start(vm.SignalRService);
        }
        else if (result.ShowError)
        {
            ShowError(result.ErrorMessage, result.IsSystemError);
        }
    }

    private async Task JoinRandomButtonClicked()
    {
        var vm = (LobbyViewModel)DataContext;

        var result = await vm.JoinRandomRoomAsync();
        if (result.ShowError)
        {
            ShowError(result.ErrorMessage, result.IsSystemError);
        }
    }

    private static void ShowError(string message, bool isSystemError)
    {
        var errorWindow = new ErrorWindow
        {
            DataContext = new ErrorViewModel(message, isSystemError),
            Topmost = true,
            CanResize = false
        };
        errorWindow.Show();
        errorWindow.Activate();
    }

    private void Start(ISignalRService signalRService)
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel(signalRService)
        };
        mainWindow.Show();
        mainWindow.Activate();

        Close();
    }
}
