using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

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

    public async Task LeaveAsync()
    {
        await _signalRService.LeaveAsync();
    }

    private ObservableCollection<UserViewModel> Users { get; } = new ObservableCollection<UserViewModel>();

    private int latency;
    private int Latency
    {
        get => latency;
        set => this.RaiseAndSetIfChanged(ref latency, value);
    }
}
