using System;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels.UserControls;

public class ConnectionStatusViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public ConnectionStatusViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        _signalRService.Pong += SignalRService_Pong;
        _signalRService.Reconnecting += SignalRService_Reconnecting;
        _signalRService.Reconnected += SignalRService_Reconnected;
    }

    private void SignalRService_Pong(object sender, PongEventArgs e)
    {
        Latency = e.Latency;
    }

    private void SignalRService_Reconnecting(object sender, EventArgs e)
    {
        Connected = false;
    }

    private void SignalRService_Reconnected(object sender, EventArgs e)
    {
        Connected = true;
    }

    private bool connected = true;
    private bool Connected
    {
        get => connected;
        set => this.RaiseAndSetIfChanged(ref connected, value);
    }

    private int latency;
    private int Latency
    {
        get => latency;
        set => this.RaiseAndSetIfChanged(ref latency, value);
    }
}
