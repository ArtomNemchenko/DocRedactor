using Microsoft.EntityFrameworkCore;
using DocRedactor.API.Data;
using DocRedactor.API.Models;
using DocRedactor.API.Repositories;

namespace DocRedactor.API.Tests.Repositories;

public class DocumentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DocumentRepository _repository;

    public DocumentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new DocumentRepository(_context);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ReturnsDocumentsForUser()
    {
        // Arrange
        var userId = "user1";
        var document1 = new Document
        {
            Title = "Doc 1",
            Content = "Content 1",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var document2 = new Document
        {
            Title = "Doc 2",
            Content = "Content 2",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        var otherUserDoc = new Document
        {
            Title = "Doc 3",
            Content = "Content 3",
            UserId = "user2",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.AddRange(document1, document2, otherUserDoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByUserIdAsync(userId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, doc => Assert.Equal(userId, doc.UserId));
        Assert.Equal("Doc 1", result.First().Title); // Most recently updated first
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDocument_WhenExists()
    {
        // Arrange
        var document = new Document
        {
            Title = "Test Doc",
            Content = "Test Content",
            UserId = "user1",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(document.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(document.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAndUserIdAsync_ReturnsDocument_WhenExistsAndUserMatches()
    {
        // Arrange
        var userId = "user1";
        var document = new Document
        {
            Title = "Test Doc",
            Content = "Test Content",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAndUserIdAsync(document.Id, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(document.Title, result.Title);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetByIdAndUserIdAsync_ReturnsNull_WhenUserDoesNotMatch()
    {
        // Arrange
        var document = new Document
        {
            Title = "Test Doc",
            Content = "Test Content",
            UserId = "user1",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAndUserIdAsync(document.Id, "user2");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsDocumentToDatabase()
    {
        // Arrange
        var document = new Document
        {
            Title = "New Doc",
            Content = "New Content",
            UserId = "user1",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(document);

        // Assert
        Assert.NotEqual(0, result.Id);
        var savedDoc = await _context.Documents.FindAsync(result.Id);
        Assert.NotNull(savedDoc);
        Assert.Equal(document.Title, savedDoc.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesDocumentInDatabase()
    {
        // Arrange
        var document = new Document
        {
            Title = "Original Title",
            Content = "Original Content",
            UserId = "user1",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // Act
        document.Title = "Updated Title";
        document.CurrentVersion = 2;
        await _repository.UpdateAsync(document);

        // Assert
        var updatedDoc = await _context.Documents.FindAsync(document.Id);
        Assert.NotNull(updatedDoc);
        Assert.Equal("Updated Title", updatedDoc.Title);
        Assert.Equal(2, updatedDoc.CurrentVersion);
        Assert.True(updatedDoc.UpdatedAt > document.CreatedAt);
    }

    [Fact]
    public async Task DeleteAsync_RemovesDocumentFromDatabase()
    {
        // Arrange
        var document = new Document
        {
            Title = "To Delete",
            Content = "Delete Me",
            UserId = "user1",
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        var documentId = document.Id;

        // Act
        await _repository.DeleteAsync(document);

        // Assert
        var deletedDoc = await _context.Documents.FindAsync(documentId);
        Assert.Null(deletedDoc);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
