using Microsoft.Extensions.Logging;
using SmartDogDoor.Services;
using SmartDogDoor.View;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;

namespace SmartDogDoor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        //Build pages
        builder.Logging.AddDebug();

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
#endif  
        builder.Services.AddSingleton<PetService>();

        builder.Services.AddSingleton<PetViewModel>();
        builder.Services.AddTransient<PetDetailsViewModel>();
        builder.Services.AddSingleton<ActivityViewModel>();
        builder.Services.AddSingleton<LockViewModel>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddSingleton<ActivityPage>();
        builder.Services.AddSingleton<LockPage>();

        return builder.Build();
    }
}
