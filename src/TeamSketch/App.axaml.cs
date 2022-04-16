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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
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
