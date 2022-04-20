using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Models;
using TeamSketch.Services;
using TeamSketch.Utils;

namespace TeamSketch.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public MainWindowViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        _signalRService.Waved += SignalRService_Waved;
        _signalRService.Joined += SignalRService_Joined;
        _signalRService.Left += SignalRService_Left;
        _signalRService.Pong += SignalRService_Pong;

        Users.Add(new UserViewModel { Nickname = _signalRService.Nickname });

        Room = _signalRService.Room;
    }

    public string Room { get; }

    private async void CopyRoom()
    {
        await Application.Current.Clipboard.SetTextAsync(Room);
    }

    private void SignalRService_Waved(object sender, UserEventArgs e)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == e.User);
        if (user == null)
        {
            Users.Add(new UserViewModel { Nickname = e.User });
        }
    }

    private void SignalRService_Joined(object sender, UserEventArgs e)
    {
        Users.Add(new UserViewModel { Nickname = e.User });
    }

    private void SignalRService_Left(object sender, UserEventArgs e)
    {
        UserLeft(e.User);
    }

    private void SignalRService_Pong(object sender, PongEventArgs e)
    {
        Latency = e.Latency;
    }

    public void InitializeUsers(List<string> others)
    {
        Users.Add(new UserViewModel { Nickname = _signalRService.Nickname });

        foreach (var user in others)
        {
            Users.Add(new UserViewModel { Nickname = user });
        }
    }

    public void UserLeft(string nickname)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == nickname);
        Users.Remove(user);
    }

    public async Task Disconnect()
    {
        await _signalRService.DisconnectAsync();
    }

    private ObservableCollection<UserViewModel> Users { get; } = new ObservableCollection<UserViewModel>();

    private ColorsEnum brushColor = ColorsEnum.Default;
    private ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            this.RaiseAndSetIfChanged(ref brushColor, value);
            BrushSettings.BrushColor = value;

            if (value == ColorsEnum.Eraser)
            {
                BrushThickness = ThicknessEnum.Eraser;
            }
            else
            {
                BrushThickness = previousBrushThickness;
            }
        }
    }

    private ThicknessEnum previousBrushThickness;
    private ThicknessEnum brushThickness;
    private ThicknessEnum BrushThickness
    {
        get => brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref brushThickness, value);
            if (BrushColor != ColorsEnum.Eraser)
            {
                previousBrushThickness = brushThickness;
            }
            BrushSettings.BrushThickness = value;
        }
    }

    private int latency;
    private int Latency
    {
        get => latency;
        set => this.RaiseAndSetIfChanged(ref latency, value);
    }
}
