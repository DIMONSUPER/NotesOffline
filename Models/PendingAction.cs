namespace NotesOffline.Models;
public class PendingAction
{
    public int Id { get; set; }

    public Guid NoteId { get; set; }

    public ActionType Action { get; set; }

    public DateTime CreatedAt { get; set; }
}

public enum ActionType
{
    None,
    Create,
    Delete,
    Update
}