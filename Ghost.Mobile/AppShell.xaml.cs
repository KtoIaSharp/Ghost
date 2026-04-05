using Ghost.Mobile.Views;
using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Services;

namespace Ghost.Mobile;

public partial class AppShell : Shell
{
    private readonly LoginPage _loginPage;
    private readonly TasksPage _tasksPage;
    private readonly DealsPage _dealsPage;
    private readonly ProfilePage _profilePage;

    public AppShell(
        LoginPage loginPage,
        TasksPage tasksPage,
        DealsPage dealsPage,
        ProfilePage profilePage)
    {
        InitializeComponent();

        _loginPage = loginPage;
        _tasksPage = tasksPage;
        _dealsPage = dealsPage;
        _profilePage = profilePage;

        // Register routes
        Routing.RegisterRoute("taskDetail", typeof(TaskDetailPage));
        Routing.RegisterRoute("createTask", typeof(CreateTaskPage));
        Routing.RegisterRoute("chat", typeof(ChatPage));

        // Login page (no tab)
        var loginItem = new ShellContent
        {
            Route = "login",
            Content = _loginPage,
            FlyoutItemIsVisible = false
        };
        Items.Add(loginItem);

        // Tab bar
        var tab = new Tab
        {
            Route = "main",
            Title = "Главная"
        };

        var tasksTab = new ShellContent
        {
            Route = "tasks",
            Title = "Задания",
            Icon = "📋",
            Content = _tasksPage
        };

        var dealsTab = new ShellContent
        {
            Route = "deals",
            Title = "Сделки",
            Icon = "💼",
            Content = _dealsPage
        };

        var profileTab = new ShellContent
        {
            Route = "profile",
            Title = "Профиль",
            Icon = "👤",
            Content = _profilePage
        };

        tab.Items.Add(tasksTab);
        tab.Items.Add(dealsTab);
        tab.Items.Add(profileTab);
        Items.Add(tab);

        // Start at login or main depending on auth
        var settings = IPlatformApplication.Current.Services.GetRequiredService<SettingsService>();
        CurrentItem = settings.IsLoggedIn ? tab : loginItem;
    }
}
