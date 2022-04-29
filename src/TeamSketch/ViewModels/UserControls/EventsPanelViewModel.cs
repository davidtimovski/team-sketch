using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public class EventsPanelViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public EventsPanelViewModel(ISignalRService signalRService)
    {
        _signalRService = signalRService;
        _signalRService.Connection.Reconnecting += Connection_Reconnecting;
        _signalRService.Connection.Reconnected += Connection_Reconnected;
        _signalRService.Connection.On<string>("JoinedRoom", Connection_JoinedRoom);
        _signalRService.Connection.On<string>("LeftRoom", Connection_LeftRoom);
    }

    public ObservableCollection<EventViewModel> Events { get; } = new();

    private Task Connection_Reconnecting(Exception arg)
    {
        Events.Add(new EventViewModel("Disconnected."));
        return Task.CompletedTask;
    }

    private Task Connection_Reconnected(string arg)
    {
        Events.Add(new EventViewModel("Reconnected."));
        return Task.CompletedTask;
    }

    private void Connection_JoinedRoom(string user)
    {
        Events.Add(new EventViewModel(user, " joined."));
    }

    private void Connection_LeftRoom(string user)
    {
        Events.Add(new EventViewModel(user, " left."));
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
