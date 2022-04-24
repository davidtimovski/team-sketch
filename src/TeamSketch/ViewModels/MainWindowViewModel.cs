using System.Collections.ObjectModel;
using System.Linq;
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

        _signalRService.Joined += SignalRService_Joined;
        _signalRService.Left += SignalRService_Left;
        _signalRService.Pong += SignalRService_Pong;

        foreach (var user in _signalRService.UsersInRoom)
        {
            Users.Add(new UserViewModel(user));
        }

        Room = _signalRService.Room;
    }

    public string Room { get; }

    public void IndicateUserDrawing(string nickname)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == nickname);
        if (user != null)
        {
            user.Drawing = true;
        }
    }

    private async void CopyRoom()
    {
        await Application.Current.Clipboard.SetTextAsync(Room);
    }

    private void SignalRService_Joined(object sender, UserEventArgs e)
    {
        Users.Add(new UserViewModel(e.User));

        Events.Add(new EventViewModel(e.User, " joined."));
    }

    private void SignalRService_Left(object sender, UserEventArgs e)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == e.User);
        Users.Remove(user);

        Events.Add(new EventViewModel(e.User, " left."));
    }

    private void SignalRService_Pong(object sender, PongEventArgs e)
    {
        Latency = e.Latency;
    }

    private ObservableCollection<UserViewModel> Users { get; } = new();
    private ObservableCollection<EventViewModel> Events { get; } = new();

    private ColorsEnum brushColor = BrushSettings.BrushColor;
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

    private ThicknessEnum previousBrushThickness = BrushSettings.BrushThickness;
    private ThicknessEnum brushThickness = BrushSettings.BrushThickness;
    private ThicknessEnum BrushThickness
    {
        get => brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref brushThickness, value);
            BrushSettings.BrushThickness = value;

            if (brushColor != ColorsEnum.Eraser || value != ThicknessEnum.Eraser)
            {
                previousBrushThickness = brushThickness;
            }
        }
    }

    private int latency;
    private int Latency
    {
        get => latency;
        set => this.RaiseAndSetIfChanged(ref latency, value);
    }
}
