using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public class UsersPanelViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public UsersPanelViewModel(ISignalRService signalRService)
    {
        _signalRService = signalRService;
        _signalRService.Connection.On<string>("JoinedRoom", Connection_JoinedRoom);
        _signalRService.Connection.On<string>("LeftRoom", Connection_LeftRoom);
    }

    public ObservableCollection<UserViewModel> Users { get; } = new();

    private void Connection_JoinedRoom(string user)
    {
        Users.Add(new UserViewModel(user));
    }

    private void Connection_LeftRoom(string nickname)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == nickname);
        Users.Remove(user);
    }
}

public class UserViewModel : ViewModelBase
{
    private readonly DispatcherTimer _drawingIndicatorTimer = new();

    public UserViewModel(string nickname)
    {
        this.nickname = nickname;

        _drawingIndicatorTimer.Tick += DrawingIndicatorTimer_Tick;
        _drawingIndicatorTimer.Interval = TimeSpan.FromSeconds(1);
    }

    private string nickname;
    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private bool drawing;
    public bool Drawing
    {
        get => drawing;
        set
        {
            this.RaiseAndSetIfChanged(ref drawing, value);

            if (!_drawingIndicatorTimer.IsEnabled)
            {
                _drawingIndicatorTimer.Start();
            }
        }
    }

    private void DrawingIndicatorTimer_Tick(object sender, EventArgs e)
    {
        Drawing = false;
        _drawingIndicatorTimer.Stop();
    }
}
