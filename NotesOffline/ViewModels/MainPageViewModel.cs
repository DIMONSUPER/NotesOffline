using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesOffline.Models.Entities;
using NotesOffline.Models.Messages;
using NotesOffline.Services;

namespace NotesOffline.ViewModels;

public partial class MainPageViewModel : BaseViewModel, IRecipient<PendingActionMessageChanged>
{
    private readonly INoteService _noteService;
    private readonly IActionService _actionService;

    public MainPageViewModel(INoteService noteService, IActionService actionService)
    {
        _noteService = noteService;
        _actionService = actionService;

        Initialize();
    }

    [ObservableProperty]
    private ObservableCollection<Note> notes;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private int messagesCount;

    public override async void OnAppearing()
    {
        base.OnAppearing();

        await UpdateNotesAsync();

        MessagesCount = await _actionService.GetActionsCountAsync();
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

    [RelayCommand]
    public Task BellIconTapped()
    {
        return Shell.Current.GoToAsync(Constants.NavigationPages.MESSAGES_PAGE, animate: true);
    }

    private async void Initialize()
    {
        Connectivity.ConnectivityChanged += OnConnectionChanged;

        IsConnected = Connectivity.NetworkAccess is NetworkAccess.Internet;

        WeakReferenceMessenger.Default.Register(this);

        MessagesCount = await _actionService.GetActionsCountAsync();
    }

    private async Task UpdateNotesAsync()
    {
        IsLoading = true;

        await _noteService.SyncWithServerAsync();

        var allNotes = await _noteService.GetAllNotesAsync();

        Notes = new(allNotes);

        IsLoading = false;
    }

    private async void OnConnectionChanged(object? sender, ConnectivityChangedEventArgs e)
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

    public async void Receive(PendingActionMessageChanged message)
    {
        MessagesCount = await _actionService.GetActionsCountAsync();
    }
}
