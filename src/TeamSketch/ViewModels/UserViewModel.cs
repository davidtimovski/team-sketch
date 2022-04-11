using ReactiveUI;

namespace TeamSketch.ViewModels;

public class UserViewModel : ViewModelBase
{
    private string nickname;
    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }
}
