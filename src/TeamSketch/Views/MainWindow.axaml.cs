using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Common;
using ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Services;
using TeamSketch.Utils;
using TeamSketch.ViewModels;

namespace TeamSketch.Views;

public partial class MainWindow : Window
{
    private readonly IRenderer _renderer;

    private Point currentPoint = new();
    private bool pressed;

    public MainWindow()
    {
        InitializeComponent();

        var signalRService = Locator.Current.GetRequiredService<ISignalRService>();
        _renderer = new Renderer(canvas, signalRService);

        signalRService.DrewPoint += SignalRService_DrewPoint;
        signalRService.DrewLine += SignalRService_DrewLine;

        canvas.PointerMoved += ThrottleHelper.CreateThrottledEventHandler(Canvas_PointerMoved, TimeSpan.FromMilliseconds(8));

        var defaultColorButton = this.FindControl<Button>("defaultColorButton");
        defaultColorButton.Command = ReactiveCommand.Create(SetDefaultColor);

        var redColorButton = this.FindControl<Button>("redColorButton");
        redColorButton.Command = ReactiveCommand.Create(SetRedColor);

        var eraserButton = this.FindControl<Button>("eraserButton");
        eraserButton.Command = ReactiveCommand.Create(SetEraser);
    }

    private void SignalRService_DrewPoint(object sender, DrewPointEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _renderer.Draw(e.Point, true);
        });
    }

    private void SignalRService_DrewLine(object sender, DrewLineEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _renderer.Draw(e.Line, true);
        });
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        currentPoint = e.GetPosition(this);
        pressed = true;
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        pressed = false;
        var point = new PointDto(currentPoint.X, currentPoint.Y, _renderer.BrushThickness, _renderer.BrushColor);
        _renderer.Draw(point);
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed)
        {
            return;
        }

        Point newPosition = e.GetPosition(this);

        var line = new LineDto(currentPoint.X, currentPoint.Y, newPosition.X, newPosition.Y, _renderer.BrushThickness, _renderer.BrushColor);
        _renderer.Draw(line);

        currentPoint = newPosition;
    }

    private void SetDefaultColor()
    {
        _renderer.SetBrush(2, ColorsEnum.Default);
    }

    private void SetRedColor()
    {
        _renderer.SetBrush(2, ColorsEnum.Red);
    }

    private void SetEraser()
    {
        _renderer.SetEraser();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        _ = vm.LeaveAsync();
    }
}
