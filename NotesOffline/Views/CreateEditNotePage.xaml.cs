using NotesOffline.ViewModels;

namespace NotesOffline.Views;

public partial class CreateEditNotePage : ContentPage
{
    public CreateEditNotePage(CreateEditNoteViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}