using Microsoft.AspNetCore.Http.HttpResults;
using NotesBackend.Contracts;
using NotesBackend.Models;
using NotesBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 3001
builder.WebHost.ConfigureKestrel(options =>
{
    // Bind to http://0.0.0.0:3001 to match container preview
    options.ListenAnyIP(3001);
});

// Add services
builder.Services.AddEndpointsApiExplorer();

// NSwag OpenAPI setup
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Notes Backend API";
    settings.Version = "v1";
    settings.Description = "RESTful API for managing personal notes.";
    settings.DocumentName = "v1";
    settings.PostProcess = document =>
    {
        // Set basic API info; omit explicit Contact object to avoid type issues
        document.Info.Title = "Notes Backend API";
        document.Info.Version = "v1";
        document.Info.Description = "RESTful API for managing personal notes.";
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register repository
builder.Services.AddSingleton<INoteRepository, InMemoryNoteRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Root health check endpoint
app.MapGet("/", () => new { message = "Healthy" })
   .WithName("HealthCheck")
   .WithSummary("Service health check")
   .WithDescription("Returns a simple object indicating the service is running.")
   .WithTags("Misc");

// Map Notes endpoints
var notesGroup = app.MapGroup("/api/notes")
    .WithTags("Notes");

// PUBLIC_INTERFACE
// Create a new note
notesGroup.MapPost("/", (CreateNoteRequest request, INoteRepository repo) =>
{
    // Basic validation as per DataAnnotations (redundant check to ensure robustness)
    if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 256)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            { nameof(request.Title), new[] { "Title is required and must be at most 256 characters." } }
        });
    }

    var now = DateTime.UtcNow;
    var note = new Note
    {
        Id = Guid.NewGuid(),
        Title = request.Title.Trim(),
        Content = request.Content ?? string.Empty,
        CreatedAt = now,
        UpdatedAt = now
    };

    repo.Create(note);

    var response = new NoteResponse
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };

    return Results.Created($"/api/notes/{note.Id}", response);
})
.WithName("CreateNote")
.WithSummary("Create a new note")
.WithDescription("Creates a new note with the provided title and content.")
.Produces<NoteResponse>(StatusCodes.Status201Created)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

// PUBLIC_INTERFACE
// List all notes
notesGroup.MapGet("/", (INoteRepository repo) =>
{
    var list = repo.GetAll()
        .Select(n => new NoteResponse
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        });

    return Results.Ok(list);
})
.WithName("ListNotes")
.WithSummary("List notes")
.WithDescription("Gets a list of all notes.")
.Produces<IEnumerable<NoteResponse>>(StatusCodes.Status200OK);

// PUBLIC_INTERFACE
// Get a single note by id
notesGroup.MapGet("/{id:guid}", (Guid id, INoteRepository repo) =>
{
    var n = repo.GetById(id);
    if (n is null)
    {
        return Results.NotFound();
    }

    var response = new NoteResponse
    {
        Id = n.Id,
        Title = n.Title,
        Content = n.Content,
        CreatedAt = n.CreatedAt,
        UpdatedAt = n.UpdatedAt
    };

    return Results.Ok(response);
})
.WithName("GetNoteById")
.WithSummary("Get note by id")
.WithDescription("Retrieves a note by its unique identifier.")
.Produces<NoteResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// PUBLIC_INTERFACE
// Update an existing note
notesGroup.MapPut("/{id:guid}", (Guid id, UpdateNoteRequest request, INoteRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 256)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            { nameof(request.Title), new[] { "Title is required and must be at most 256 characters." } }
        });
    }

    var existing = repo.GetById(id);
    if (existing is null)
    {
        return Results.NotFound();
    }

    existing.Title = request.Title.Trim();
    existing.Content = request.Content ?? string.Empty;
    existing.UpdatedAt = DateTime.UtcNow;

    var updated = repo.Update(existing);
    if (updated is null)
    {
        return Results.NotFound();
    }

    var response = new NoteResponse
    {
        Id = updated.Id,
        Title = updated.Title,
        Content = updated.Content,
        CreatedAt = updated.CreatedAt,
        UpdatedAt = updated.UpdatedAt
    };

    return Results.Ok(response);
})
.WithName("UpdateNote")
.WithSummary("Update note")
.WithDescription("Updates the title and content of an existing note.")
.Produces<NoteResponse>(StatusCodes.Status200OK)
.ProducesValidationProblem(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound);

// PUBLIC_INTERFACE
// Delete a note
notesGroup.MapDelete("/{id:guid}", (Guid id, INoteRepository repo) =>
{
    var deleted = repo.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteNote")
.WithSummary("Delete note")
.WithDescription("Deletes a note by its id.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();