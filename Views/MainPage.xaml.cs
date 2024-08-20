using NotesOffline.ViewModels;

namespace NotesOffline.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        BindingContext = mainPageViewModel;
        _viewModel = mainPageViewModel;

        InitializeComponent();
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        _viewModel.OnAppearing();
    }
}

