using Microsoft.Extensions.Logging;
using SmartDogDoor.Services;
using SmartDogDoor.View;
using CommunityToolkit.Maui;

namespace SmartDogDoor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        //Build pages
        builder.Logging.AddDebug();
#endif  
        builder.Services.AddSingleton<PetService>();

        builder.Services.AddSingleton<PetViewModel>();
        builder.Services.AddTransient<PetDetailsViewModel>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailsPage>();

        return builder.Build();
    }
}
