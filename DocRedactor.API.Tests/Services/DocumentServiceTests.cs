using Microsoft.Extensions.Logging;
using NSubstitute;
using DocRedactor.API.DTOs;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;
using DocRedactor.API.Services;

namespace DocRedactor.API.Tests.Services;

public class DocumentServiceTests
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _versionRepository;
    private readonly ILogger<DocumentService> _logger;
    private readonly DocumentService _service;

    public DocumentServiceTests()
    {
        _documentRepository = Substitute.For<IDocumentRepository>();
        _versionRepository = Substitute.For<IDocumentVersionRepository>();
        _logger = Substitute.For<ILogger<DocumentService>>();
        _service = new DocumentService(_documentRepository, _versionRepository, _logger);
    }

    [Fact]
    public async Task GetAllDocumentsByUserAsync_CallsRepositoryAndReturnsDocuments()
    {
        // Arrange
        var userId = "user1";
        var documents = new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Doc 1",
                Content = "Content 1",
                UserId = userId,
                CurrentVersion = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Document
            {
                Id = 2,
                Title = "Doc 2",
                Content = "Content 2",
                UserId = userId,
                CurrentVersion = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _documentRepository.GetAllByUserIdAsync(userId).Returns(documents);

        // Act
        var result = await _service.GetAllDocumentsByUserAsync(userId);

        // Assert
        await _documentRepository.Received(1).GetAllByUserIdAsync(userId);
        Assert.Equal(2, result.Count());
        Assert.Equal("Doc 1", result.First().Title);
    }

    [Fact]
    public async Task GetDocumentByIdAsync_CallsRepositoryAndReturnsDocument()
    {
        // Arrange
        var documentId = 1;
        var userId = "user1";
        var document = new Document
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Test Content",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns(document);

        // Act
        var result = await _service.GetDocumentByIdAsync(documentId, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        Assert.NotNull(result);
        Assert.Equal("Test Doc", result.Title);
    }

    [Fact]
    public async Task GetDocumentByIdAsync_ReturnsNull_WhenDocumentNotFound()
    {
        // Arrange
        var documentId = 999;
        var userId = "user1";
        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns((Document?)null);

        // Act
        var result = await _service.GetDocumentByIdAsync(documentId, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateDocumentAsync_CallsRepositoriesAndReturnsDocument()
    {
        // Arrange
        var userId = "user1";
        var createDto = new CreateDocumentDto
        {
            Title = "New Doc",
            Content = "New Content"
        };

        var createdDocument = new Document
        {
            Id = 1,
            Title = createDto.Title,
            Content = createDto.Content,
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentRepository.CreateAsync(Arg.Any<Document>()).Returns(createdDocument);

        // Act
        var result = await _service.CreateDocumentAsync(createDto, userId);

        // Assert
        await _documentRepository.Received(1).CreateAsync(Arg.Any<Document>());
        await _versionRepository.Received(1).CreateAsync(Arg.Any<DocumentVersion>());
        Assert.Equal("New Doc", result.Title);
        Assert.Equal(1, result.CurrentVersion);
    }

    [Fact]
    public async Task UpdateDocumentAsync_CallsRepositoriesAndUpdatesDocument()
    {
        // Arrange
        var documentId = 1;
        var userId = "user1";
        var updateDto = new UpdateDocumentDto
        {
            Content = "Updated Content",
            ChangeDescription = "Updated"
        };

        var existingDocument = new Document
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Original Content",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns(existingDocument);

        // Act
        var result = await _service.UpdateDocumentAsync(documentId, updateDto, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        await _versionRepository.Received(1).CreateAsync(Arg.Any<DocumentVersion>());
        await _documentRepository.Received(1).UpdateAsync(Arg.Any<Document>());
        Assert.NotNull(result);
        Assert.Equal("Updated Content", result.Content);
        Assert.Equal(2, result.CurrentVersion);
    }

    [Fact]
    public async Task UpdateDocumentAsync_ReturnsNull_WhenDocumentNotFound()
    {
        // Arrange
        var documentId = 999;
        var userId = "user1";
        var updateDto = new UpdateDocumentDto
        {
            Content = "Updated Content"
        };

        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns((Document?)null);

        // Act
        var result = await _service.UpdateDocumentAsync(documentId, updateDto, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        await _versionRepository.Received(0).CreateAsync(Arg.Any<DocumentVersion>());
        await _documentRepository.Received(0).UpdateAsync(Arg.Any<Document>());
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteDocumentAsync_CallsRepositoryAndReturnsTrue()
    {
        // Arrange
        var documentId = 1;
        var userId = "user1";
        var document = new Document
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Test Content",
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns(document);

        // Act
        var result = await _service.DeleteDocumentAsync(documentId, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        await _documentRepository.Received(1).DeleteAsync(document);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteDocumentAsync_ReturnsFalse_WhenDocumentNotFound()
    {
        // Arrange
        var documentId = 999;
        var userId = "user1";
        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns((Document?)null);

        // Act
        var result = await _service.DeleteDocumentAsync(documentId, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        await _documentRepository.Received(0).DeleteAsync(Arg.Any<Document>());
        Assert.False(result);
    }

    [Fact]
    public async Task RevertToVersionAsync_CallsRepositoriesAndCreatesNewVersion()
    {
        // Arrange
        var documentId = 1;
        var userId = "user1";
        var revertDto = new RevertToVersionDto
        {
            VersionNumber = 1,
            ChangeDescription = "Reverted"
        };

        var document = new Document
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Current Content",
            UserId = userId,
            CurrentVersion = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var versionToRevert = new DocumentVersion
        {
            Id = 1,
            DocumentId = documentId,
            Content = "Original Content",
            VersionNumber = 1,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _documentRepository.GetByIdAndUserIdAsync(documentId, userId).Returns(document);
        _versionRepository.GetVersionByDocumentAndVersionNumberAsync(documentId, 1).Returns(versionToRevert);

        // Act
        var result = await _service.RevertToVersionAsync(documentId, revertDto, userId);

        // Assert
        await _documentRepository.Received(1).GetByIdAndUserIdAsync(documentId, userId);
        await _versionRepository.Received(1).GetVersionByDocumentAndVersionNumberAsync(documentId, 1);
        await _versionRepository.Received(1).CreateAsync(Arg.Any<DocumentVersion>());
        await _documentRepository.Received(1).UpdateAsync(Arg.Any<Document>());
        Assert.NotNull(result);
        Assert.Equal("Original Content", result.Content);
        Assert.Equal(3, result.CurrentVersion);
    }
}
