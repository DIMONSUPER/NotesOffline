using NotesOffline.Views;

namespace NotesOffline;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(Constants.NavigationPages.CREATE_EDIT_NOTE_PAGE, typeof(CreateEditNotePage));
    }
}
