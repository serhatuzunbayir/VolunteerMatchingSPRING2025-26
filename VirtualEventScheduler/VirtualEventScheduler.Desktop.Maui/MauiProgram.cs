using VirtualEventScheduler.Desktop.Maui.Pages;
using VirtualEventScheduler.Desktop.Maui.Services;

namespace VirtualEventScheduler.Desktop.Maui
{
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

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
