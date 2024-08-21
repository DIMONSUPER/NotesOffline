using NotesOffline.Views;

namespace NotesOffline;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(Constants.NavigationPages.CREATE_EDIT_NOTE_PAGE, typeof(CreateEditNotePage));
        Routing.RegisterRoute(Constants.NavigationPages.MESSAGES_PAGE, typeof(MessagesPage));
    }
}
