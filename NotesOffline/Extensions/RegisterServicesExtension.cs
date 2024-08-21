using NotesOffline.Services;

namespace NotesOffline.Extensions;
public static class RegisterServicesExtension
{
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IRestService, RestService>();
        builder.Services.AddTransient<IActionService, ActionService>();
        builder.Services.AddTransient<INoteService, NoteService>();

        return builder;
    }
}
