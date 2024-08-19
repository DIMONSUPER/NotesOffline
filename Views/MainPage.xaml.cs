using NotesOffline.ViewModels;

namespace NotesOffline;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        BindingContext = mainPageViewModel;

        InitializeComponent();
    }
}

