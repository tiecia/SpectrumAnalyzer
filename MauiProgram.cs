using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AudioLoopbackTest;

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
            }).UseMauiCommunityToolkit().UseSkiaSharp();

        builder.Services.AddTransient<SettingsPopup>();
        builder.Services.AddSingleton<Settings>();
        
        // Allows IServiceProvider to be injected into it
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<WaterfallView>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}