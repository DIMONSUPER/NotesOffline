using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesOffline.DataAccess;
using NotesOffline.DataAccess.Entities;

namespace NotesOffline.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var notes = await _context.Set<Note>()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(notes);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string noteId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(noteId, out var guid))
        {
            return BadRequest();
        }

        var noteToDelete = await _context.Set<Note>()
            .FirstOrDefaultAsync(x => x.Id == guid, cancellationToken);

        if (noteToDelete is null)
        {
            return BadRequest();
        }

        noteToDelete.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(noteToDelete);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Note note, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Set<Note>().AddAsync(note, cancellationToken);

        entry.Entity.CreatedAt = entry.Entity.CreatedAt?.ToUniversalTime() ?? DateTime.UtcNow;
        entry.Entity.EditedAt = entry.Entity.EditedAt?.ToUniversalTime();

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(entry.Entity);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Note note, CancellationToken cancellationToken = default)
    {
        var noteFromDb = await _context.Set<Note>().FirstAsync(x => x.Id == note.Id, cancellationToken);

        noteFromDb.Title = note.Title;
        noteFromDb.Content = note.Content;
        noteFromDb.EditedAt = note.EditedAt?.ToUniversalTime() ?? DateTime.UtcNow;
        noteFromDb.CreatedAt = noteFromDb.CreatedAt?.ToUniversalTime();

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(noteFromDb);
    }
}
