using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.Common;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels;

public class EnterViewModel : ViewModelBase
{
    private readonly ISignalRService _signalRService;

    public EnterViewModel(bool fromMainWindow = false)
    {
        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();

        if (fromMainWindow)
        {
            joinTabVisible = true;
            nickname = _signalRService.Nickname;
            room = _signalRService.Room;

            _signalRService.ClearEventHandlers();
        }
    }

    public async Task<EnterValidationResult> CreateRoomAsync()
    {
        Entering = true;

        if (nickname.Trim().Length == 0)
        {
            NicknameIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, null, false, false);
        }

        var error = Validations.ValidateNickname(nickname);
        if (error != null)
        {
            NicknameIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, error, true, false);
        }

        try
        {
            await _signalRService.CreateRoomAsync(nickname);
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not connect to the server. Please check your internet connection.", true, true);
        }
    }

    public async Task<EnterValidationResult> JoinRoomAsync()
    {
        Entering = true;
        NicknameIsInvalid = RoomIsInvalid = false;

        if (nickname.Trim().Length == 0)
        {
            NicknameIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, null, false, false);
        }

        if (room.Trim().Length == 0)
        {
            RoomIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, null, false, false);
        }

        var nicknameError = Validations.ValidateNickname(nickname);
        if (nicknameError != null)
        {
            NicknameIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, nicknameError, true, false);
        }

        var roomError = Validations.ValidateRoomName(room);
        if (roomError != null)
        {
            RoomIsInvalid = true;
            Entering = false;
            return new EnterValidationResult(false, roomError, true, false);
        }

        try
        {
            var validationResult = await HttpProxy.ValidateJoinRoomAsync(room, nickname);
            if (!validationResult.RoomExists)
            {
                RoomIsInvalid = true;
                Entering = false;
                return new EnterValidationResult(false, "Room does not exist.", true, false);
            }

            if (validationResult.RoomIsFull)
            {
                RoomIsInvalid = true;
                Entering = false;
                return new EnterValidationResult(false, "Room is at capacity.", true, false);
            }

            if (validationResult.NicknameIsTaken)
            {
                RoomIsInvalid = true;
                Entering = false;
                return new EnterValidationResult(false, "The nickname is taken in that room.", true, false);
            }


            await _signalRService.JoinRoomAsync(nickname, room);
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not connect to the server. Please check your internet connection.", true, true);
        }
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

public readonly record struct EnterValidationResult(bool Success, string ErrorMessage, bool ShowError, bool IsSystemError);
