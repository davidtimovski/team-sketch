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
        _signalRService.DrewPoint += SignalRService_DrewPoint;
        _signalRService.DrewLine += SignalRService_DrewLine;

        canvas.PointerMoved += ThrottleHelper.CreateThrottledEventHandler(Canvas_PointerMoved, TimeSpan.FromMilliseconds(8));
    }

    private void SignalRService_DrewPoint(object sender, DrewEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var point = PayloadConverter.ToPoint(e.Data);
            canvas.Children.Add(point);
        });
    }

    private void SignalRService_DrewLine(object sender, DrewEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var shapes = PayloadConverter.ToLineShapes(e.Data);
            canvas.Children.AddRange(shapes);
        });
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
            _ = _signalRService.DrawPointAsync(x, y, BrushSettings.Thickness, BrushSettings.BrushColor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
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
            _ = _signalRService.DrawLineAsync(currentPoint.X, currentPoint.Y, x, y, BrushSettings.Thickness, BrushSettings.BrushColor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        currentPoint = new Point(x, y);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        _ = vm.Disconnect();

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
