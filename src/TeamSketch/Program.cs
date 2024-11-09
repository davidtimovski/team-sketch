using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using TeamSketch.DependencyInjection;

namespace TeamSketch;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Bootstrapper.Register(Locator.CurrentMutable);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    } 

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
