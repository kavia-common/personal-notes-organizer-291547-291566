using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Models
{
    /// <summary>
    /// Domain model representing a Note.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Unique identifier for the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content of the note, can be empty.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// When the note was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the note was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
