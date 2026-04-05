namespace Ghost.Mobile;

public partial class App : Application
{
    public App(IServiceProvider provider)
    {
        InitializeComponent();

        var shell = provider.GetRequiredService<AppShell>();
        MainPage = shell;
    }
}
