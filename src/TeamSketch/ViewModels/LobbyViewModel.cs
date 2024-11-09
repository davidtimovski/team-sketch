using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TeamSketch.Common;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.Utils;

namespace TeamSketch.ViewModels;

public sealed class LobbyViewModel : ReactiveObject
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

    public async Task ExitRandomRoomQueueAsync()
    {
        Entering = false;

        try
        {
            await SignalRService.Connection.StopAsync();
        }
        catch
        {
            Entering = false;
        }
    }

    public void SelectTab(object tab)
    {
        if (Entering)
        {
            return;
        }

        switch (tab)
        {
            case "create":
                JoinTabSelected = JoinRandomTabSelected = false;
                CreateTabSelected = true;
                break;
            case "join":
                CreateTabSelected = JoinRandomTabSelected = false;
                JoinTabSelected = true;
                break;
            case "random":
                CreateTabSelected = JoinTabSelected = false;
                JoinRandomTabSelected = true;
                break;
        }
    }

    private bool createTabSelected;
    public bool CreateTabSelected
    {
        get => createTabSelected;
        set => this.RaiseAndSetIfChanged(ref createTabSelected, value);
    }

    private bool joinTabSelected;
    public bool JoinTabSelected
    {
        get => joinTabSelected;
        set => this.RaiseAndSetIfChanged(ref joinTabSelected, value);
    }

    private bool joinRandomTabSelected;
    public bool JoinRandomTabSelected
    {
        get => joinRandomTabSelected;
        set => this.RaiseAndSetIfChanged(ref joinRandomTabSelected, value);
    }

    private string nickname = string.Empty;
    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private bool nicknameIsInvalid;
    public bool NicknameIsInvalid
    {
        get => nicknameIsInvalid;
        set => this.RaiseAndSetIfChanged(ref nicknameIsInvalid, value);
    }

    private string room = string.Empty;
    public string Room
    {
        get => room;
        set => this.RaiseAndSetIfChanged(ref room, value);
    }

    private bool roomIsInvalid;
    public bool RoomIsInvalid
    {
        get => roomIsInvalid;
        set => this.RaiseAndSetIfChanged(ref roomIsInvalid, value);
    }

    private string createButtonLabel = "Create";
    public string CreateButtonLabel
    {
        get => createButtonLabel;
        set => this.RaiseAndSetIfChanged(ref createButtonLabel, value);
    }

    private string joinButtonLabel = "Join";
    public string JoinButtonLabel
    {
        get => joinButtonLabel;
        set => this.RaiseAndSetIfChanged(ref joinButtonLabel, value);
    }

    private string joinRandomButtonLabel = "Join random";
    public string JoinRandomButtonLabel
    {
        get => joinRandomButtonLabel;
        set => this.RaiseAndSetIfChanged(ref joinRandomButtonLabel, value);
    }

    private bool entering;
    public bool Entering
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
