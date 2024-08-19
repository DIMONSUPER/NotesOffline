using Microsoft.EntityFrameworkCore;
using NotesOffline.Data;

namespace NotesOffline.Extensions;

public static class RegisterDbContextExtension
{
    public static MauiAppBuilder RegisterDbContext(this MauiAppBuilder builder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DB_NAME);
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Filename={dbPath}"));

        using var scope = builder.Services.BuildServiceProvider();
        var context = scope.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        return builder;
    }
}
