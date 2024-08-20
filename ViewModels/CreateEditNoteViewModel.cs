using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesOffline.Models;
using NotesOffline.Resources.Strings;
using NotesOffline.Services;

namespace NotesOffline.ViewModels;

public partial class CreateEditNoteViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INoteService _noteService;

    public CreateEditNoteViewModel(INoteService noteService)
    {
        _noteService = noteService;

        UpdateTitle();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(SelectedNote), out var param) && param is Note note)
        {
            SelectedNote = note;
        }
    }

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    public Note selectedNote = new();

    [RelayCommand]
    public async Task SaveButtonTapped()
    {
        Note? result;

        if (SelectedNote.Id is 0)
        {
            result = await _noteService.CreateNoteAsync(SelectedNote);
        }
        else
        {
            result = await _noteService.UpdateNoteAsync(SelectedNote);
        }

        if (result is null)
        {
            await Shell.Current.DisplayAlert(Strings.ErrorDuringSaving, Strings.NoteSaveFailed, Strings.Ok);
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    public async Task DeleteButtonTapped()
    {
        if (SelectedNote.Id is 0)
        {
            await _noteService.CreateNoteAsync(SelectedNote);
        }
        else
        {
            await _noteService.UpdateNoteAsync(SelectedNote);
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public Task BackButtonTapped()
    {
        return Shell.Current.GoToAsync("..");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(SelectedNote))
        {
            UpdateTitle();
        }
    }

    private void UpdateTitle()
    {
        Title = SelectedNote.Id is 0 ? Strings.CreateNote : Strings.EditNote;
    }
}
