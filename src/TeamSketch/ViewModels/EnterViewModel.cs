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

        if (_signalRService.Nickname != null)
        {
            nickname = _signalRService.Nickname;
        }
    }

    public async Task<bool> CreateRoomAsync()
    {
        if (!FormIsValid())
        {
            return false;
        }

        try
        {
            await _signalRService.CreateRoomAsync(nickname);
            return true;
        }
        catch (Exception ex)
        {
            // TODO
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> JoinRoomAsync()
    {
        if (!FormIsValid())
        {
            return false;
        }

        try
        {
            await _signalRService.JoinRoomAsync(nickname, room);
            return true;
        }
        catch (Exception ex)
        {
            // TODO
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private bool FormIsValid()
    {
        var valid = true;

        if (nickname.Trim().Length == 0)
        {
            valid = false;
        }

        if (room.Trim().Length == 0)
        {
            valid = false;
        }

        return valid;
    }

    private void ToggleTab()
    {
        JoinTabVisible = !JoinTabVisible;
    }

    private bool joinTabVisible;
    private bool JoinTabVisible
    {
        get => joinTabVisible;
        set => this.RaiseAndSetIfChanged(ref joinTabVisible, value);
    }

    private string nickname = string.Empty;
    private string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private string room = string.Empty;
    private string Room
    {
        get => room;
        set => this.RaiseAndSetIfChanged(ref room, value);
    }
}
