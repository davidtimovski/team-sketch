using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.Utils;

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
        Entering = true;

        if (!ValidateNickname())
        {
            Entering = false;
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
            Entering = false;
            return false;
        }
    }

    public async Task<bool> JoinRoomAsync()
    {
        Entering = true;
        NicknameIsInvalid = RoomIsInvalid = false;

        bool valid = ValidateNickname();

        if (room.Trim().Length != 7 || !ValidationUtil.IsAlphanumeric(room))
        {
            RoomIsInvalid = true;
            valid = false;
        }

        if (!valid)
        {
            Entering = false;
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
            Entering = false;
            return false;
        }
    }

    private bool ValidateNickname()
    {
        if (nickname.Trim().Length < 2)
        {
            NicknameIsInvalid = true;
            return false;
        }

        if (!ValidationUtil.IsAlphanumeric(nickname))
        {
            NicknameIsInvalid = true;
            return false;
        }

        return true;
    }

    private void ToggleTab()
    {
        if (Entering)
        {
            return;
        }

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

    private bool nicknameIsInvalid;
    private bool NicknameIsInvalid
    {
        get => nicknameIsInvalid;
        set => this.RaiseAndSetIfChanged(ref nicknameIsInvalid, value);
    }

    private string room = string.Empty;
    private string Room
    {
        get => room;
        set => this.RaiseAndSetIfChanged(ref room, value);
    }

    private bool roomIsInvalid;
    private bool RoomIsInvalid
    {
        get => roomIsInvalid;
        set => this.RaiseAndSetIfChanged(ref roomIsInvalid, value);
    }

    private string createButtonLabel = "Create";
    private string CreateButtonLabel
    {
        get => createButtonLabel;
        set => this.RaiseAndSetIfChanged(ref createButtonLabel, value);
    }

    private string joinButtonLabel = "Join";
    private string JoinButtonLabel
    {
        get => joinButtonLabel;
        set => this.RaiseAndSetIfChanged(ref joinButtonLabel, value);
    }

    private bool entering;
    private bool Entering
    {
        get => entering;
        set
        {
            this.RaiseAndSetIfChanged(ref entering, value);
            CreateButtonLabel = value ? "Creating" : "Create";
            JoinButtonLabel = value ? "Joining" : "Join";
        }
    }
}
