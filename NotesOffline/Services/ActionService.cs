using Microsoft.EntityFrameworkCore;
using NotesOffline.Models.Entities;
using NotesOffline.Data;
using CommunityToolkit.Mvvm.Messaging;
using NotesOffline.Models.Messages;
using NotesOffline.ViewModels;

namespace NotesOffline.Services;

public interface IActionService
{
    Task CreateNewActionForNoteAsync(Note note, ActionType actionType, CancellationToken cancellationToken = default);

    Task<List<PendingAction>> GetAllActionsAsync(CancellationToken cancellationToken = default);

    Task RemoveActionAsync(int actionId, CancellationToken cancellationToken = default);

    Task<int> GetActionsCountAsync(CancellationToken cancellationToken = default);
}

public class ActionService : IActionService
{
    private readonly ApplicationDbContext _context;

    public ActionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateNewActionForNoteAsync(Note note, ActionType actionType, CancellationToken cancellationToken = default)
    {
        var pendingAction = new PendingAction
        {
            NoteId = note.Id,
            Action = actionType,
            CreatedAt = DateTime.UtcNow,
        };

        await _context.Set<PendingAction>().AddAsync(pendingAction, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        WeakReferenceMessenger.Default.Send(new PendingActionMessageChanged(pendingAction, ActionChangedType.Added));
    }

    public Task<List<PendingAction>> GetAllActionsAsync(CancellationToken cancellationToken = default)
    {
        return _context.Set<PendingAction>()
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveActionAsync(int actionId, CancellationToken cancellationToken = default)
    {
        var action = await _context.Set<PendingAction>().FirstAsync(x => x.Id == actionId, cancellationToken);

        _context.Set<PendingAction>().Remove(action);

        await _context.SaveChangesAsync(cancellationToken);

        WeakReferenceMessenger.Default.Send(new PendingActionMessageChanged(action, ActionChangedType.Deleted));
    }

    public Task<int> GetActionsCountAsync(CancellationToken cancellationToken = default)
    {
        return _context.Set<PendingAction>().CountAsync(cancellationToken);
    }
}