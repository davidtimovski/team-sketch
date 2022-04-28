using System;
using System.Collections.ObjectModel;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public class EventsPanelViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public EventsPanelViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        _signalRService.Reconnecting += SignalRService_Reconnecting;
        _signalRService.Reconnected += SignalRService_Reconnected;
        _signalRService.UserJoined += SignalRService_UserJoined;
        _signalRService.UserLeft += SignalRService_UserLeft;
    }

    public ObservableCollection<EventViewModel> Events { get; } = new();

    private void SignalRService_Reconnecting(object sender, EventArgs e)
    {
        Events.Add(new EventViewModel("Disconnected."));
    }

    private void SignalRService_Reconnected(object sender, EventArgs e)
    {
        Events.Add(new EventViewModel("Reconnected."));
    }

    private void SignalRService_UserJoined(object sender, UserEventArgs e)
    {
        Events.Add(new EventViewModel(e.User, " joined."));
    }

    private void SignalRService_UserLeft(object sender, UserEventArgs e)
    {
        Events.Add(new EventViewModel(e.User, " left."));
    }
}

public class EventViewModel : ViewModelBase
{
    public EventViewModel(string eventMessage)
    {
        EventMessage = eventMessage;
    }

    public EventViewModel(string user, string eventMessage)
    {
        HasUser = true;
        User = user;
        EventMessage = eventMessage;
    }

    public bool HasUser { get; private set; }
    public string User { get; private set; }
    public string EventMessage { get; private set; }
}
