using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels;

public class EnterViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;
    private ReactiveCommand<Unit, Unit> OnToggleTabCommand { get; }

    public EnterViewModel()
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        OnToggleTabCommand = ReactiveCommand.Create(ToggleTab);
    }

    private void ToggleTab()
    {
        JoinTabVisible = !JoinTabVisible;
    }

    public async Task<string> CreateRoomAsync()
    {
        try
        {
            await _signalRService.CreateRoomAsync(nickname);
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> JoinRoomAsync()
    {
        try
        {
            await _signalRService.JoinRoomAsync(nickname, room);
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private bool joinTabVisible;
    private bool JoinTabVisible
    {
        get => joinTabVisible;
        set => this.RaiseAndSetIfChanged(ref joinTabVisible, value);
    }

    private string nickname;
    private string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private string room;
    private string Room
    {
        get => room;
        set => this.RaiseAndSetIfChanged(ref room, value);
    }
}
