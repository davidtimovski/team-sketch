namespace TeamSketch.ViewModels;

public class ErrorViewModel : ViewModelBase
{
    public ErrorViewModel(string message, bool isSystemError)
    {
        Title = isSystemError ? "Whoops!" : "Sorry, but..";
        Message = message;
    }

    public string Title { get; private set; }
    public string Message { get; private set; }
}
