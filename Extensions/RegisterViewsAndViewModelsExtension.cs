using NotesOffline.ViewModels;

namespace NotesOffline.Extensions;
public static class RegisterViewsAndViewModelsExtension
{
    public static MauiAppBuilder RegisterViewsAndViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();

        return builder;
    }
}
