using Microsoft.Extensions.Logging;
using Ghost.Mobile.Services;
using Ghost.Mobile.Views;
using Ghost.Mobile.ViewModels;

namespace Ghost.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Системные шрифты
            });

        // Services
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<AuthService>();

        // ViewModels
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<TasksViewModel>();
        builder.Services.AddTransient<TaskDetailViewModel>();
        builder.Services.AddTransient<CreateTaskViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddSingleton<DealsViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();

        // Pages — все Singleton для стабильности
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<TasksPage>();
        builder.Services.AddSingleton<TaskDetailPage>();
        builder.Services.AddSingleton<CreateTaskPage>();
        builder.Services.AddSingleton<ChatPage>();
        builder.Services.AddSingleton<DealsPage>();
        builder.Services.AddSingleton<ProfilePage>();

        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
