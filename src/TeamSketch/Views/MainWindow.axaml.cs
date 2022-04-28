using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.Utils;
using TeamSketch.ViewModels;

namespace TeamSketch.Views;

public partial class MainWindow : Window
{
    private readonly IRenderer _renderer;
    private readonly ISignalRService _signalRService;
    private Action onCloseAction; 

    private Point currentPoint = new();
    private bool pressed;

    public MainWindow()
    {
        InitializeComponent();

        _renderer = new Renderer(canvas);

        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();
        _signalRService.Disconnected += SignalRService_Disconnected;
        _signalRService.UserDrewPoint += SignalRService_UserDrewPoint;
        _signalRService.UserDrewLine += SignalRService_UserDrewLine;

        canvas.Cursor = BrushSettings.Cursor;
        canvas.PointerMoved += ThrottleHelper.CreateThrottledEventHandler(Canvas_PointerMoved, TimeSpan.FromMilliseconds(8));

        BrushSettings.BrushChanged += BrushSettings_BrushChanged;

        onCloseAction = NormalClose;
    }

    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
    }

    private void SignalRService_Disconnected(object sender, EventArgs e)
    {
        onCloseAction = CloseFromDisconnect;

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Close();
        });
    }

    private void SignalRService_UserDrewPoint(object sender, DrewEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var point = PayloadConverter.ToPoint(e.Data);
            canvas.Children.Add(point);
        });

        IndicateUserDrawing(e.User);
    }

    private void SignalRService_UserDrewLine(object sender, DrewEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var shapes = PayloadConverter.ToLineShapes(e.Data);
            canvas.Children.AddRange(shapes);
        });

        IndicateUserDrawing(e.User);
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        currentPoint = e.GetPosition(canvas);
        pressed = true;
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        pressed = false;

        var (x, y) = _renderer.RestrictPointToCanvas(currentPoint.X, currentPoint.Y);
         _renderer.DrawPoint(x, y);

        _ = _signalRService.DrawPointAsync(x, y);

        IndicateUserDrawing(_signalRService.Nickname);
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed)
        {
            return;
        }

        Point newPosition = e.GetPosition(canvas);
        var (x, y) = _renderer.RestrictPointToCanvas(newPosition.X, newPosition.Y);

        _renderer.DrawLine(currentPoint.X, currentPoint.Y, x, y);

        _ = _signalRService.DrawLineAsync(currentPoint.X, currentPoint.Y, x, y);

        currentPoint = new Point(x, y);

        IndicateUserDrawing(_signalRService.Nickname);
    }

    private void IndicateUserDrawing(string nickname)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.IndicateUserDrawing(nickname);
    }

    private void NormalClose()
    {
        _ = _signalRService.DisconnectAsync();

        var window = new EnterWindow
        {
            DataContext = new EnterViewModel(true),
            Topmost = true,
            CanResize = false
        };
        window.Show();
    }

    private void CloseFromDisconnect()
    {
        var enterWindow = new EnterWindow
        {
            DataContext = new EnterViewModel(true),
            Topmost = true,
            CanResize = false
        };
        enterWindow.Show();

        var errorWindow = new ErrorWindow
        {
            DataContext = new ErrorViewModel("You got disconnected :( Please check your internet connection.", true),
            Topmost = true,
            CanResize = false
        };
        errorWindow.Show();
        errorWindow.Activate();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        onCloseAction();
    }
}
