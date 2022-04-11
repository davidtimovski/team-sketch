using System;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels;

public class EnterViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public EnterViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();
    }

    public async Task<string> ConnectAsync()
    {
        try
        {
            await _signalRService.ConnectAsync(nickname, room);
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string nickname = "John";
    private string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private string room = "Silly goose";
    private string Room
    {
        get => room;
        set => this.RaiseAndSetIfChanged(ref room, value);
    }
}
