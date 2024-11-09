using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public sealed class EventsPanelViewModel : ReactiveObject
{
    public EventsPanelViewModel(ISignalRService signalRService)
    {
        signalRService.Connection.Reconnecting += Connection_Reconnecting;
        signalRService.Connection.Reconnected += Connection_Reconnected;
        signalRService.Connection.On<string>("JoinedRoom", Connection_JoinedRoom);
        signalRService.Connection.On<string>("LeftRoom", Connection_LeftRoom);
    }

    public ObservableCollection<EventViewModel> Events { get; } = [];

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

    private void Connection_JoinedRoom(string participant)
    {
        Events.Add(new EventViewModel(participant, " joined."));
    }

    private void Connection_LeftRoom(string participant)
    {
        Events.Add(new EventViewModel(participant, " left."));
    }
}

public class EventViewModel : ReactiveObject
{
    public EventViewModel(string eventMessage)
    {
        EventMessage = eventMessage;
    }

    public EventViewModel(string participant, string eventMessage)
    {
        HasParticipant = true;
        Participant = participant;
        EventMessage = eventMessage;
    }

    public bool HasParticipant { get; private set; }
    public string Participant { get; private set; }
    public string EventMessage { get; private set; }
}
