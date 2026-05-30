using VirtualEventScheduler.Desktop.Maui.Pages;
using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui
{
    /// <summary>
    /// Entry point for the MAUI app on macOS (and Windows if built with the windows target).
    /// Registers all pages and services via dependency injection.
    /// </summary>
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register ApiService as singleton so the JWT token persists across pages
            builder.Services.AddSingleton<ApiService>();

            // Register all pages as transient (new instance each navigation)
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ParticipantsPage>();

            return builder.Build();
        }
    }
}
