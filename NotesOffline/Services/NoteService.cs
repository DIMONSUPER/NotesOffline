using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using NotesOffline.Data;
using NotesOffline.Models.Entities;

namespace NotesOffline.Services;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllNotesAsync(CancellationToken cancellationToken = default);
    Task<Note?> CreateNoteAsync(Note newNote, CancellationToken cancellationToken = default);
    Task<Note?> UpdateNoteAsync(Note updatedNote, CancellationToken cancellationToken = default);
    Task<bool> DeleteNoteAsync(Note noteToDelete, CancellationToken cancellationToken = default);
    Task SyncWithServerAsync(CancellationToken cancellationToken = default);
}

public class NoteService : INoteService
{
    private const string NOTES_API_URL = "https://192.168.1.222:7276/api/notes";

    private readonly ApplicationDbContext _context;
    private readonly IRestService _restService;
    private readonly IActionService _actionService;

    public NoteService(ApplicationDbContext context, IRestService restService, IActionService actionService)
    {
        _context = context;
        _restService = restService;
        _actionService = actionService;
    }

    public bool IsConnected => Connectivity.NetworkAccess is NetworkAccess.Internet;

    public async Task<IEnumerable<Note>> GetAllNotesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Note> result = [];

        try
        {
            result = await _context.Set<Note>()
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    public async Task<Note?> CreateNoteAsync(Note newNote, CancellationToken cancellationToken = default)
    {
        Note? result = null;

        var onlineNote = await TryCreateOnlineNoteAsync(newNote, cancellationToken);

        try
        {
            if (onlineNote is not null)
            {
                newNote = onlineNote;
            }
            else
            {
                newNote.CreatedAt = DateTime.UtcNow;
            }

            await _context.Set<Note>().AddAsync(newNote, cancellationToken);

            result = newNote;

            if (onlineNote is null)
            {
                await _actionService.CreateNewActionForNoteAsync(result, ActionType.Create, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    public async Task<Note?> UpdateNoteAsync(Note updatedNote, CancellationToken cancellationToken = default)
    {
        Note? result = null;

        var onlineNote = await TryUpdateOnlineNoteAsync(updatedNote, cancellationToken);

        try
        {
            var note = await _context.Set<Note>().AsTracking().FirstAsync(x => x.Id == updatedNote.Id, cancellationToken);

            note.Content = updatedNote.Content;
            note.Title = updatedNote.Title;
            note.EditedAt = DateTime.UtcNow;

            if (onlineNote is null)
            {
                await _actionService.CreateNewActionForNoteAsync(note, ActionType.Update, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            result = updatedNote;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    public async Task<bool> DeleteNoteAsync(Note noteToDelete, CancellationToken cancellationToken = default)
    {
        bool result;

        var onlineNote = await TryDeleteOnlineNoteAsync(noteToDelete, cancellationToken);

        try
        {
            var note = await _context.Set<Note>().AsTracking().FirstAsync(x => x.Id == noteToDelete.Id, cancellationToken);

            if (onlineNote is null)
            {
                await _actionService.CreateNewActionForNoteAsync(note, ActionType.Delete, cancellationToken);
            }

            _context.Set<Note>().Remove(note);

            await _context.SaveChangesAsync(cancellationToken);

            result = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            result = false;
        }

        return result;
    }

    public async Task SyncWithServerAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return;
        }

        try
        {
            var pendingActions = await _actionService.GetAllActionsAsync(cancellationToken);

            foreach (var action in pendingActions)
            {
                await TrySyncNoteAsync(action, cancellationToken);
            }

            var onlineNotes = await TryGetAllOnlineNotesAsync(cancellationToken);

            if (onlineNotes is not null)
            {
                await _context.BulkInsertOrUpdateAsync(onlineNotes, cancellationToken: cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async Task TrySyncNoteAsync(PendingAction action, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return;
        }

        try
        {
            Note? note = null;

            switch (action.Action)
            {
                case ActionType.Delete:
                    {
                        note = await _restService.DeleteAsync<Note>($"{NOTES_API_URL}?noteId={action.NoteId}");
                        break;
                    }
                case ActionType.Create:
                    {
                        note = await _context.Set<Note>().AsNoTracking().FirstAsync(x => x.Id == action.NoteId, cancellationToken);

                        await _restService.PostAsync<Note>(NOTES_API_URL, note);
                        break;
                    }
                case ActionType.Update:
                    {
                        note = await _context.Set<Note>().AsNoTracking().FirstAsync(x => x.Id == action.NoteId, cancellationToken);

                        await _restService.PutAsync<Note>(NOTES_API_URL, note);
                        break;
                    }
            }

            await _actionService.RemoveActionAsync(action.Id, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async Task<List<Note>?> TryGetAllOnlineNotesAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return null;
        }

        List<Note>? result = null;

        try
        {
            result = await _restService.GetAsync<List<Note>?>(NOTES_API_URL);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    private async Task<Note?> TryDeleteOnlineNoteAsync(Note noteToDelete, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return null;
        }

        Note? result = null;

        try
        {
            result = await _restService.DeleteAsync<Note>($"{NOTES_API_URL}?noteId=${noteToDelete.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    private async Task<Note?> TryCreateOnlineNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return null;
        }

        Note? result = null;

        try
        {
            result = await _restService.PostAsync<Note>(NOTES_API_URL, note);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    private async Task<Note?> TryUpdateOnlineNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            return null;
        }

        Note? result = null;

        try
        {
            result = await _restService.PutAsync<Note>(NOTES_API_URL, note);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }
}
