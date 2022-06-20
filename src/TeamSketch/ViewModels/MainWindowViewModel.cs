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
        participantsPanel = new ParticipantsPanelViewModel(signalRService);
        eventsPanel = new EventsPanelViewModel(signalRService);
        connectionStatus = new ConnectionStatusViewModel(signalRService);

        _ = GetParticipantsInRoomAsync();
    }

    public ISignalRService SignalRService { get; }
    public string Room { get; }

    public void IndicateDrawing(string nickname)
    {
        ParticipantViewModel participant = ParticipantsPanel.Participants.FirstOrDefault(x => x.Nickname == nickname);
        if (participant != null)
        {
            participant.Drawing = true;
        }
    }

    private async Task GetParticipantsInRoomAsync()
    {
        var participants = await HttpProxy.GetParticipantsAsync(Room);

        foreach (var participantNickname in participants)
        {
            ParticipantsPanel.Participants.Add(new ParticipantViewModel(participantNickname));
        }

        var initialEventMessage = participants.Count == 1 ? "Room created." : "Joined room.";
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

    private ParticipantsPanelViewModel participantsPanel;
    private ParticipantsPanelViewModel ParticipantsPanel
    {
        get => participantsPanel;
        set => this.RaiseAndSetIfChanged(ref participantsPanel, value);
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
