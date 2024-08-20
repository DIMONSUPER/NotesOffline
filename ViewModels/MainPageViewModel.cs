using System.Collections.ObjectModel;
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
    }

    [ObservableProperty]
    private ObservableCollection<Note> notes;

    [ObservableProperty]
    private bool isLoading;

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

        var allNotes = await _noteService.GetAllNotesAsync();

        Notes = new(allNotes);

        IsLoading = false;
    }
}
