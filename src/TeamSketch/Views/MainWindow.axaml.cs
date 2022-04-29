using System;
using System.ComponentModel;
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
    private Point currentPoint = new();
    private bool pressed;
    private Action closeAdditionalAction = () => { };
    private bool isClosing;

    public MainWindow()
    {
        InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();
        _renderer = new Renderer(_appState.BrushSettings, canvas);

        canvas.Cursor = _appState.BrushSettings.Cursor;
        canvas.PointerMoved += ThrottleHelper.CreateThrottledEventHandler(Canvas_PointerMoved, TimeSpan.FromMilliseconds(8));

        _appState.BrushSettings.BrushChanged += BrushSettings_BrushChanged;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.SignalRService.Connection.On<string, byte[]>("DrewPoint", Connection_UserDrewPoint);
        vm.SignalRService.Connection.On<string, byte[]>("DrewLine", Connection_UserDrewLine);
        vm.SignalRService.Connection.Closed += Connection_Closed;

        base.OnDataContextChanged(e);
    }

    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
    }

    private void Connection_UserDrewPoint(string user, byte[] data)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var point = PayloadConverter.ToPoint(data);
            canvas.Children.Add(point);
        });

        IndicateUserDrawing(user);
    }

    private void Connection_UserDrewLine(string user, byte[] data)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var shapes = PayloadConverter.ToLineShapes(data);
            canvas.Children.AddRange(shapes);
        });

        IndicateUserDrawing(user);
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

        var vm = DataContext as MainWindowViewModel;
        _ = vm.SignalRService.DrawPointAsync(x, y);

        IndicateUserDrawing(_appState.Nickname);
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

        var vm = DataContext as MainWindowViewModel;
        _ = vm.SignalRService.DrawLineAsync(currentPoint.X, currentPoint.Y, x, y);

        currentPoint = new Point(x, y);

        IndicateUserDrawing(_appState.Nickname);
    }

    private void IndicateUserDrawing(string nickname)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.IndicateUserDrawing(nickname);
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

        var window = new EnterWindow
        {
            DataContext = new EnterViewModel(true),
            Topmost = true,
            CanResize = false
        };
        window.Show();

        closeAdditionalAction();
    }
}
