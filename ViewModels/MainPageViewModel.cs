using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesOffline.Models;
using NotesOffline.Services;

namespace NotesOffline.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly INoteService _noteService;

    public MainPageViewModel(INoteService noteService)
    {
        _noteService = noteService;

        Connectivity.ConnectivityChanged += OnConnectionChanged;

        IsConnected = Connectivity.NetworkAccess is NetworkAccess.Internet;
    }

    [ObservableProperty]
    private ObservableCollection<Note> notes;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isConnected;

    public override async void OnAppearing()
    {
        base.OnAppearing();

        await UpdateNotesAsync();
    }

    [RelayCommand]
    public Task ItemTapped(Note selectedNote)
    {
        var parameters = new Dictionary<string, object>
        {
            {nameof(CreateEditNoteViewModel.SelectedNote), selectedNote}
        };

        return Shell.Current.GoToAsync(Constants.NavigationPages.CREATE_EDIT_NOTE_PAGE, parameters:parameters, animate: true);
    }

    [RelayCommand]
    public Task AddNoteButtonTapped()
    {
        return Shell.Current.GoToAsync(Constants.NavigationPages.CREATE_EDIT_NOTE_PAGE, animate: true);
    }

    private async Task UpdateNotesAsync()
    {
        IsLoading = true;

        await _noteService.SyncWithServerAsync();

        var allNotes = await _noteService.GetAllNotesAsync();

        Notes = new(allNotes);

        IsLoading = false;
    }

    public async void OnConnectionChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        var newIsConnected = e.NetworkAccess is NetworkAccess.Internet;

        if (newIsConnected != IsConnected)
        {
            IsConnected = newIsConnected;

            if (IsConnected)
            {
                await UpdateNotesAsync();
            }
        }
    }
}
