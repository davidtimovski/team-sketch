using ReactiveUI;

namespace TeamSketch.ViewModels;

public sealed class ErrorViewModel : ReactiveObject
{
    public ErrorViewModel(string message, bool isSystemError)
    {
        Title = isSystemError ? "Whoops!" : "Sorry, but..";
        Message = message;
    }

    public string Title { get; private set; }
    public string Message { get; private set; }
}
