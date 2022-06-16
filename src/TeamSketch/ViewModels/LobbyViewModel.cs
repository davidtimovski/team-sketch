using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.Common;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;

namespace TeamSketch.ViewModels;

public class LobbyViewModel : ViewModelBase
{
    private readonly IAppState _appState;

    public LobbyViewModel(bool fromMainWindow = false)
    {
        _appState = Locator.Current.GetRequiredService<IAppState>();
        SignalRService = new SignalRService(_appState);

        if (fromMainWindow)
        {
            joinTabSelected = true;
            nickname = _appState.Nickname;
            room = _appState.Room;
        }
        else
        {
            createTabSelected = true;
        }
    }

    public ISignalRService SignalRService { get; }

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
            _appState.Nickname = nickname.Trim();
            await SignalRService.CreateRoomAsync();
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not connect to the server. Please check your internet connection or try again later.", true, true);
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

            _appState.Nickname = nickname.Trim();
            _appState.Room = room.Trim();
            await SignalRService.JoinRoomAsync();
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not connect to the server. Please check your internet connection or try again later.", true, true);
        }
    }

    public async Task<EnterValidationResult> JoinRandomRoomAsync()
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
            _appState.Nickname = nickname.Trim();
            await SignalRService.JoinRandomRoomAsync();
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not connect to the server. Please check your internet connection or try again later.", true, true);
        }
    }

    private async Task<EnterValidationResult> ExitRandomRoomQueueAsync()
    {
        Entering = false;

        try
        {
            await SignalRService.Connection.StopAsync();
            return new EnterValidationResult(true, null, false, false);
        }
        catch
        {
            Entering = false;
            return new EnterValidationResult(false, "Could not talk to the server. Please check your internet connection or try again later.", true, true);
        }
    }

    private void SelectTab(int tab)
    {
        if (Entering)
        {
            return;
        }

        switch (tab)
        {
            case 0:
                JoinTabSelected = JoinRandomTabSelected = false;
                CreateTabSelected = true;
                break;
            case 1:
                CreateTabSelected = JoinRandomTabSelected = false;
                JoinTabSelected = true;
                break;
            case 2:
                CreateTabSelected = JoinTabSelected = false;
                JoinRandomTabSelected = true;
                break;
        }
    }


    private bool createTabSelected;
    private bool CreateTabSelected
    {
        get => createTabSelected;
        set => this.RaiseAndSetIfChanged(ref createTabSelected, value);
    }

    private bool joinTabSelected;
    private bool JoinTabSelected
    {
        get => joinTabSelected;
        set => this.RaiseAndSetIfChanged(ref joinTabSelected, value);
    }

    private bool joinRandomTabSelected;
    private bool JoinRandomTabSelected
    {
        get => joinRandomTabSelected;
        set => this.RaiseAndSetIfChanged(ref joinRandomTabSelected, value);
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

    private string joinRandomButtonLabel = "Join random";
    private string JoinRandomButtonLabel
    {
        get => joinRandomButtonLabel;
        set => this.RaiseAndSetIfChanged(ref joinRandomButtonLabel, value);
    }

    private bool entering;
    private bool Entering
    {
        get => entering;
        set
        {
            this.RaiseAndSetIfChanged(ref entering, value);
            CreateButtonLabel = value && createTabSelected ? "Creating" : "Create";
            JoinButtonLabel = value && joinTabSelected ? "Joining" : "Join";
            JoinRandomButtonLabel = value && joinRandomTabSelected ? "In queue" : "Join random";
        }
    }
}

public readonly record struct EnterValidationResult(bool Success, string ErrorMessage, bool ShowError, bool IsSystemError);
