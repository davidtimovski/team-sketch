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
    
    private Point currentPoint = new();
    private bool pressed;

    public MainWindow()
    {
        InitializeComponent();

        _renderer = new Renderer(canvas);

        _signalRService = Locator.Current.GetRequiredService<ISignalRService>();
        _signalRService.UserDrewPoint += SignalRService_UserDrewPoint;
        _signalRService.UserDrewLine += SignalRService_UserDrewLine;

        canvas.Cursor = BrushSettings.Cursor;
        canvas.PointerMoved += ThrottleHelper.CreateThrottledEventHandler(Canvas_PointerMoved, TimeSpan.FromMilliseconds(8));

        BrushSettings.BrushChanged += BrushSettings_BrushChanged;
    }

    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
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

        try
        {
            _ = _signalRService.DrawPointAsync(x, y);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

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

        try
        {
            _ = _signalRService.DrawLineAsync(currentPoint.X, currentPoint.Y, x, y);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        currentPoint = new Point(x, y);

        IndicateUserDrawing(_signalRService.Nickname);
    }

    private void IndicateUserDrawing(string nickname)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.IndicateUserDrawing(nickname);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _ = _signalRService.DisconnectAsync();

        var window = new EnterWindow
        {
            DataContext = new EnterViewModel(),
            Topmost = true,
            CanResize = false
        };
        window.Show();
        window.Activate();
    }
}
