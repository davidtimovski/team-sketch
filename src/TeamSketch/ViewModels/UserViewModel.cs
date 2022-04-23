using System;
using Avalonia.Threading;
using ReactiveUI;

namespace TeamSketch.ViewModels;

public class UserViewModel : ViewModelBase
{
    private readonly DispatcherTimer _drawingIndicatorTimer = new();

    public UserViewModel(string nickname)
    {
        this.nickname = nickname;

        _drawingIndicatorTimer.Tick += DrawingIndicatorTimer_Tick;
        _drawingIndicatorTimer.Interval = TimeSpan.FromSeconds(1);
    }

    private string nickname;
    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private bool drawing;
    public bool Drawing
    {
        get => drawing;
        set
        {
            this.RaiseAndSetIfChanged(ref drawing, value);

            if (!_drawingIndicatorTimer.IsEnabled)
            {
                _drawingIndicatorTimer.Start();
            }
        }
    }

    private void DrawingIndicatorTimer_Tick(object sender, EventArgs e)
    {
        Drawing = false;
        _drawingIndicatorTimer.Stop();
    }
}
