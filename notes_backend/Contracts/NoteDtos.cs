using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Contracts
{
    /// <summary>
    /// Request body for creating a note.
    /// </summary>
    public class CreateNoteRequest
    {
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for updating a note.
    /// </summary>
    public class UpdateNoteRequest
    {
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for a note.
    /// </summary>
    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
