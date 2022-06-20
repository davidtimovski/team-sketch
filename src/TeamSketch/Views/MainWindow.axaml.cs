using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Splat;
using TeamSketch.DependencyInjection;
using TeamSketch.Models;
using TeamSketch.Services;
using TeamSketch.Utils;
using TeamSketch.ViewModels;

namespace TeamSketch.Views;

public partial class MainWindow : Window
{
    private readonly IAppState _appState;
    private readonly IRenderer _renderer;
    private readonly DispatcherTimer _lineRenderingTimer = new();
    private Point currentPoint = new();
    private bool pressed;
    private Action closeAdditionalAction = () => { };
    private bool isClosing;


    public MainWindow()
    {
        InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();
        _renderer = new Renderer(_appState.BrushSettings, canvas);

        _lineRenderingTimer.Tick += LineRenderingTimer_Tick;
        _lineRenderingTimer.Interval = TimeSpan.FromMilliseconds(Globals.RenderingIntervalMs);
        _lineRenderingTimer.Start();

        canvas.Cursor = _appState.BrushSettings.Cursor;
        canvas.PointerMoved += Canvas_PointerMoved;

        _appState.BrushSettings.BrushChanged += BrushSettings_BrushChanged;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.SignalRService.Connection.On<string, byte[]>("DrewPoint", Connection_ParticipantDrewPoint);
        vm.SignalRService.Connection.On<string, byte[]>("DrewLine", Connection_ParticipantDrewLine);
        vm.SignalRService.Connection.Closed += Connection_Closed;

        base.OnDataContextChanged(e);
    }

    private void LineRenderingTimer_Tick(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var points = _renderer.RenderLine();
            if (!points.Any())
            {
                return;
            }

            var vm = DataContext as MainWindowViewModel;
            _ = vm.SignalRService.DrawLineAsync(points);
        });
    }

    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
    }

    private void Connection_ParticipantDrewPoint(string participant, byte[] data)
    {
        var point = PayloadConverter.ToPoint(data);

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            canvas.Children.Add(point);
        });

        IndicateDrawing(participant);
    }

    private void Connection_ParticipantDrewLine(string participant, byte[] data)
    {
        var (points, thickness, colorBrush) = PayloadConverter.ToLine(data);

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _renderer.RenderLine(points, thickness, colorBrush);
        });

        IndicateDrawing(participant);
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        currentPoint = e.GetPosition(canvas);
        pressed = true;
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        pressed = false;

        var newPoint = _renderer.RestrictPointToCanvas(currentPoint.X, currentPoint.Y);
        _renderer.DrawPoint(newPoint.X, newPoint.Y);

        var vm = DataContext as MainWindowViewModel;
        _ = vm.SignalRService.DrawPointAsync(newPoint.X, newPoint.Y);

        IndicateDrawing(_appState.Nickname);
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed)
        {
            return;
        }

        Point newPosition = e.GetPosition(canvas);
        var newPoint = _renderer.RestrictPointToCanvas(newPosition.X, newPosition.Y);

        _renderer.EnqueueLineSegment(currentPoint, newPoint);

        currentPoint = newPoint;

        IndicateDrawing(_appState.Nickname);
    }

    private void IndicateDrawing(string nickname)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.IndicateDrawing(nickname);
    }

    private Task Connection_Closed(Exception arg)
    {
        if (isClosing)
        {
            return Task.CompletedTask;
        }

        closeAdditionalAction = () =>
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm.SignalRService.Connection.State == HubConnectionState.Disconnected)
            {
                var errorWindow = new ErrorWindow
                {
                    DataContext = new ErrorViewModel("You got disconnected :( Please check your internet connection or try again later.", true),
                    Topmost = true,
                    CanResize = false
                };
                errorWindow.Show();
                errorWindow.Activate();
            }
        };

        Dispatcher.UIThread.InvokeAsync(Close);

        return Task.CompletedTask;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        isClosing = true;

        var vm = DataContext as MainWindowViewModel;
        if (vm.SignalRService.Connection.State == HubConnectionState.Connected)
        {
            _ = vm.SignalRService.Connection.StopAsync();
        }
        _ = vm.SignalRService.Connection.DisposeAsync();

        var window = new LobbyWindow
        {
            DataContext = new LobbyViewModel(true),
            Topmost = true,
            CanResize = false
        };
        window.Show();

        closeAdditionalAction();
    }
}
