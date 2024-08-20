using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NotesOffline.Extensions;

namespace NotesOffline;
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
		builder.Logging.AddDebug();
#endif

        builder.RegisterServices();
        builder.RegisterViewsAndViewModels();
        builder.RegisterDbContext();

        return builder.Build();
    }
}
