using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.ViewModels.UserControls;

namespace TeamSketch.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAppState _appState;

    public MainWindowViewModel(ISignalRService signalRService)
    {
        _appState = Locator.Current.GetRequiredService<IAppState>();
        SignalRService = signalRService;

        Room = _appState.Room;

        toolsPanel = new ToolsPanelViewModel(_appState.BrushSettings);
        usersPanel = new UsersPanelViewModel(signalRService);
        eventsPanel = new EventsPanelViewModel(signalRService);
        connectionStatus = new ConnectionStatusViewModel(signalRService);

        _ = GetUsersInRoomAsync();
    }

    public ISignalRService SignalRService { get; }
    public string Room { get; }

    public void IndicateUserDrawing(string nickname)
    {
        UserViewModel user = UsersPanel.Users.FirstOrDefault(x => x.Nickname == nickname);
        if (user != null)
        {
            user.Drawing = true;
        }
    }

    private async Task GetUsersInRoomAsync()
    {
        var usersInRoom = await HttpProxy.GetUsersInRoomAsync(Room);

        foreach (var user in usersInRoom)
        {
            UsersPanel.Users.Add(new UserViewModel(user));
        }

        var initialEventMessage = usersInRoom.Count == 1 ? "Room created." : "Joined room.";
        EventsPanel.Events.Add(new EventViewModel(initialEventMessage));
    }

    private async void CopyRoom()
    {
        await Application.Current.Clipboard.SetTextAsync(Room);
    }

    private ToolsPanelViewModel toolsPanel;
    private ToolsPanelViewModel ToolsPanel
    {
        get => toolsPanel;
        set => this.RaiseAndSetIfChanged(ref toolsPanel, value);
    }

    private UsersPanelViewModel usersPanel;
    private UsersPanelViewModel UsersPanel
    {
        get => usersPanel;
        set => this.RaiseAndSetIfChanged(ref usersPanel, value);
    }

    private EventsPanelViewModel eventsPanel;
    private EventsPanelViewModel EventsPanel
    {
        get => eventsPanel;
        set => this.RaiseAndSetIfChanged(ref eventsPanel, value);
    }

    private ConnectionStatusViewModel connectionStatus;
    private ConnectionStatusViewModel ConnectionStatus
    {
        get => connectionStatus;
        set => this.RaiseAndSetIfChanged(ref connectionStatus, value);
    }
}
