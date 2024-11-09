using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public sealed class ConnectionStatusViewModel : ReactiveObject
{
    private readonly ISignalRService _signalRService;
    private readonly DispatcherTimer _pingTimer = new();
    private DateTime lastPing;

    public ConnectionStatusViewModel(ISignalRService signalRService)
    {
        _signalRService = signalRService;
        _signalRService.Connection.On("Pong", () =>
        {
            TimeSpan diff = DateTime.Now - lastPing;
            Latency = diff.Milliseconds;
        });
        _signalRService.Connection.Reconnecting += Connection_Reconnecting;
        _signalRService.Connection.Reconnected += Connection_Reconnected;
        _signalRService.Connection.Closed += Connection_Closed;

        _pingTimer.Tick += PingTimer_Tick;
        _pingTimer.Interval = TimeSpan.FromSeconds(3);
        _pingTimer.Start();
    }

    private Task Connection_Reconnecting(Exception arg)
    {
        _pingTimer.Stop();

        Connected = false;

        return Task.CompletedTask;
    }

    private Task Connection_Reconnected(string arg)
    {
        _pingTimer.Start();

        Connected = true;

        return Task.CompletedTask;
    }

    private Task Connection_Closed(Exception arg)
    {
        _pingTimer.Stop();
        return Task.CompletedTask;
    }

    private async void PingTimer_Tick(object sender, EventArgs e)
    {
        lastPing = DateTime.Now;
        await _signalRService.Connection.InvokeAsync("Ping");
    }

    private bool connected = true;
    public bool Connected
    {
        get => connected;
        set => this.RaiseAndSetIfChanged(ref connected, value);
    }

    private int latency;
    public int Latency
    {
        get => latency;
        set => this.RaiseAndSetIfChanged(ref latency, value);
    }

    private string reconnectingLabel = "Reconnecting";
    public string ReconnectingLabel
    {
        get => reconnectingLabel;
        set => this.RaiseAndSetIfChanged(ref reconnectingLabel, value);
    }
}
