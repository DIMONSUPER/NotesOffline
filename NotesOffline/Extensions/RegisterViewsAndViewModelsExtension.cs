using NotesOffline.ViewModels;
using NotesOffline.Views;

namespace NotesOffline.Extensions;
public static class RegisterViewsAndViewModelsExtension
{
    public static MauiAppBuilder RegisterViewsAndViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();

        builder.Services.AddTransient<CreateEditNotePage>();
        builder.Services.AddTransient<CreateEditNoteViewModel>();

        builder.Services.AddTransient<MessagesPage>();
        builder.Services.AddTransient<MessagesViewModel>();

        return builder;
    }
}
