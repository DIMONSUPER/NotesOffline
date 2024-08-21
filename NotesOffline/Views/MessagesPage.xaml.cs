using NotesOffline.ViewModels;

namespace NotesOffline.Views;

public partial class MessagesPage : ContentPage
{
	private readonly BaseViewModel _viewModel;
	public MessagesPage(MessagesViewModel viewModel)
	{
		BindingContext = viewModel;
        _viewModel = viewModel;

        InitializeComponent();
	}

    private void OnAppearing(object? sender, EventArgs e)
    {
        _viewModel.OnAppearing();
    }
}