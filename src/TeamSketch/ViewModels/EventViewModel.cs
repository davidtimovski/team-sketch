namespace TeamSketch.ViewModels;

public class EventViewModel : ViewModelBase
{
    public EventViewModel(string user, string eventMessage)
    {
        User = user;
        EventMessage = eventMessage;
    }

    public string User { get; private set; }
    public string EventMessage { get; private set; }
}
