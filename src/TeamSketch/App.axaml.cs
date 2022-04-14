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

    private IClassicDesktopStyleApplicationLifetime _desktop;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;

            var window = new EnterWindow
            {
                DataContext = new EnterViewModel(),
                Topmost = true,
                CanResize = false
            };
            window.Show();
            window.Activate();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
