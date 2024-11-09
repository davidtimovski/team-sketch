using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public sealed class ParticipantsPanelViewModel : ReactiveObject
{
    public ParticipantsPanelViewModel(ISignalRService signalRService)
    {
        signalRService.Connection.On<string>("JoinedRoom", Connection_JoinedRoom);
        signalRService.Connection.On<string>("LeftRoom", Connection_LeftRoom);
    }

    public ObservableCollection<ParticipantViewModel> Participants { get; } = [];

    private void Connection_JoinedRoom(string nickname)
    {
        Participants.Add(new ParticipantViewModel(nickname));
    }

    private void Connection_LeftRoom(string nickname)
    {
        ParticipantViewModel participant = Participants.FirstOrDefault(x => x.Nickname == nickname);
        Participants.Remove(participant);
    }
}

public sealed class ParticipantViewModel : ReactiveObject
{
    private readonly DispatcherTimer _drawingIndicatorTimer = new();

    public ParticipantViewModel(string nickname)
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
