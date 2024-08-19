using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesOffline.Data;
using NotesOffline.Models;

namespace NotesOffline.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly ApplicationDbContext _context;

    public MainPageViewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [ObservableProperty]
    private int count;

    [RelayCommand]
    public async Task Increase()
    {
        var notes = _context.Set<Note>().ToList();

        if (notes.Count < 1) 
        {
            await _context.AddAsync(new Note { Title ="New note", Content = "Some content here", IsSynced = false });
            await _context.SaveChangesAsync();
        }
    }
}
