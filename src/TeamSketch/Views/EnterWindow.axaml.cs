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

public partial class EnterWindow : Window
{
    private readonly IAppState _appState;

    public EnterWindow()
    {
        InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();

        var createButton = this.FindControl<Button>("createButton");
        createButton.Command = ReactiveCommand.Create(CreateButtonClicked);

        var joinButton = this.FindControl<Button>("joinButton");
        joinButton.Command = ReactiveCommand.Create(JoinButtonClicked);

        var joinRandomButton = this.FindControl<Button>("joinRandomButton");
        joinRandomButton.Command = ReactiveCommand.Create(JoinRandomButtonClicked);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        var vm = (EnterViewModel)DataContext;

        vm.SignalRService.Connection.On<string>("RandomRoomJoined", (room) =>
        {
            _appState.Room = room;
            Start(vm.SignalRService);
        });

        base.OnDataContextChanged(e);
    }

    private async Task CreateButtonClicked()
    {
        var vm = (EnterViewModel)DataContext;

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
        var vm = (EnterViewModel)DataContext;

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
        var vm = (EnterViewModel)DataContext;

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
