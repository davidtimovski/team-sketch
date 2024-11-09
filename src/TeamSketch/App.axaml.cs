using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TeamSketch.ViewModels;
using TeamSketch.Views;

namespace TeamSketch;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Startup += Startup;

            var window = new LobbyWindow
            {
                DataContext = new LobbyViewModel(),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void Startup(object _, ControlledApplicationLifetimeStartupEventArgs e)
    {
        if (e.Args.Length > 0)
        {
            Globals.RenderingIntervalMs = short.Parse(e.Args[0]);
        }
    }
}
