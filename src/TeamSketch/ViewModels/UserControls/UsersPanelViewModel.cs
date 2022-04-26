using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public class UsersPanelViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public UsersPanelViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        _signalRService.UserJoined += SignalRService_UserJoined;
        _signalRService.UserLeft += SignalRService_UserLeft;
    }

    public ObservableCollection<UserViewModel> Users { get; } = new();

    private void SignalRService_UserJoined(object sender, UserEventArgs e)
    {
        Users.Add(new UserViewModel(e.User));
    }

    private void SignalRService_UserLeft(object sender, UserEventArgs e)
    {
        UserViewModel user = Users.FirstOrDefault(x => x.Nickname == e.User);
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
