using NotesBackend.Models;

namespace NotesBackend.Repositories
{
    /// <summary>
    /// Repository abstraction for working with Note entities.
    /// </summary>
    public interface INoteRepository
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Create a new note.
        /// </summary>
        /// <param name="note">Note to create.</param>
        /// <returns>The created note.</returns>
        Note Create(Note note);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get all notes.
        /// </summary>
        /// <returns>Enumerable of notes.</returns>
        IEnumerable<Note> GetAll();

        // PUBLIC_INTERFACE
        /// <summary>
        /// Get a note by its id.
        /// </summary>
        /// <param name="id">Note id.</param>
        /// <returns>The note if found; otherwise null.</returns>
        Note? GetById(Guid id);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Update an existing note.
        /// </summary>
        /// <param name="note">Note with new values.</param>
        /// <returns>Updated note or null if not found.</returns>
        Note? Update(Note note);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Delete a note by id.
        /// </summary>
        /// <param name="id">Note id.</param>
        /// <returns>True if deleted, false if not found.</returns>
        bool Delete(Guid id);
    }
}
