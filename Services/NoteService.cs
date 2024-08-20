using NotesOffline.Data;
using NotesOffline.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesOffline.Services;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllNotesAsync(CancellationToken cancellationToken = default);
    Task<Note?> CreateNoteAsync(Note newNote, CancellationToken cancellationToken = default);
    Task<Note?> UpdateNoteAsync(Note updatedNote, CancellationToken cancellationToken = default);
    Task<bool> DeleteNoteAsync(Note noteToDelete, CancellationToken cancellationToken = default);
}

public class NoteService : INoteService
{
    private readonly ApplicationDbContext _context;

    public NoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Note>> GetAllNotesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Note> result = [];

        try
        {
            result = await _context.Set<Note>().ToListAsync(cancellationToken);
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

        try
        {
            var entry = await _context.Set<Note>().AddAsync(newNote, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            result = entry.Entity;
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

        try
        {
            var entry = _context.Set<Note>().Update(updatedNote);

            await _context.SaveChangesAsync(cancellationToken);

            result = entry.Entity;
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

        try
        {
            _context.Set<Note>().Remove(noteToDelete);

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
}
