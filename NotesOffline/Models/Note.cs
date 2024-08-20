namespace NotesOffline.Models;
public class Note
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
