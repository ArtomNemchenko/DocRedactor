using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;
using DocRedactor.API.Controllers;
using DocRedactor.API.DTOs;
using DocRedactor.API.Interfaces;

namespace DocRedactor.API.Tests.Controllers;

public class DocumentsControllerTests
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;
    private readonly DocumentsController _controller;
    private const string TestUserId = "test-user-123";

    public DocumentsControllerTests()
    {
        _documentService = Substitute.For<IDocumentService>();
        _logger = Substitute.For<ILogger<DocumentsController>>();
        _controller = new DocumentsController(_documentService, _logger);

        // Setup user context
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserId)
        }, "test"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetDocuments_ReturnsOkWithDocuments()
    {
        // Arrange
        var documents = new List<DocumentResponseDto>
        {
            new DocumentResponseDto
            {
                Id = 1,
                Title = "Doc 1",
                Content = "Content 1",
                UserId = TestUserId,
                CurrentVersion = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _documentService.GetAllDocumentsByUserAsync(TestUserId).Returns(documents);

        // Act
        var result = await _controller.GetDocuments();

        // Assert
        await _documentService.Received(1).GetAllDocumentsByUserAsync(TestUserId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDocs = Assert.IsAssignableFrom<IEnumerable<DocumentResponseDto>>(okResult.Value);
        Assert.Single(returnedDocs);
    }

    [Fact]
    public async Task GetDocument_ReturnsOkWithDocument_WhenExists()
    {
        // Arrange
        var documentId = 1;
        var document = new DocumentResponseDto
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Test Content",
            UserId = TestUserId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentService.GetDocumentByIdAsync(documentId, TestUserId).Returns(document);

        // Act
        var result = await _controller.GetDocument(documentId);

        // Assert
        await _documentService.Received(1).GetDocumentByIdAsync(documentId, TestUserId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDoc = Assert.IsType<DocumentResponseDto>(okResult.Value);
        Assert.Equal("Test Doc", returnedDoc.Title);
    }

    [Fact]
    public async Task GetDocument_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        var documentId = 999;
        _documentService.GetDocumentByIdAsync(documentId, TestUserId).Returns((DocumentResponseDto?)null);

        // Act
        var result = await _controller.GetDocument(documentId);

        // Assert
        await _documentService.Received(1).GetDocumentByIdAsync(documentId, TestUserId);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateDocument_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateDocumentDto
        {
            Title = "New Doc",
            Content = "New Content"
        };

        var createdDocument = new DocumentResponseDto
        {
            Id = 1,
            Title = createDto.Title,
            Content = createDto.Content,
            UserId = TestUserId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentService.CreateDocumentAsync(createDto, TestUserId).Returns(createdDocument);

        // Act
        var result = await _controller.CreateDocument(createDto);

        // Assert
        await _documentService.Received(1).CreateDocumentAsync(createDto, TestUserId);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedDoc = Assert.IsType<DocumentResponseDto>(createdResult.Value);
        Assert.Equal("New Doc", returnedDoc.Title);
        Assert.Equal(nameof(DocumentsController.GetDocument), createdResult.ActionName);
    }

    [Fact]
    public async Task UpdateDocument_ReturnsOkWithDocument_WhenSuccessful()
    {
        // Arrange
        var documentId = 1;
        var updateDto = new UpdateDocumentDto
        {
            Content = "Updated Content",
            ChangeDescription = "Updated"
        };

        var updatedDocument = new DocumentResponseDto
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Updated Content",
            UserId = TestUserId,
            CurrentVersion = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentService.UpdateDocumentAsync(documentId, updateDto, TestUserId).Returns(updatedDocument);

        // Act
        var result = await _controller.UpdateDocument(documentId, updateDto);

        // Assert
        await _documentService.Received(1).UpdateDocumentAsync(documentId, updateDto, TestUserId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDoc = Assert.IsType<DocumentResponseDto>(okResult.Value);
        Assert.Equal("Updated Content", returnedDoc.Content);
        Assert.Equal(2, returnedDoc.CurrentVersion);
    }

    [Fact]
    public async Task UpdateDocument_ReturnsNotFound_WhenDocumentDoesNotExist()
    {
        // Arrange
        var documentId = 999;
        var updateDto = new UpdateDocumentDto
        {
            Content = "Updated Content"
        };

        _documentService.UpdateDocumentAsync(documentId, updateDto, TestUserId).Returns((DocumentResponseDto?)null);

        // Act
        var result = await _controller.UpdateDocument(documentId, updateDto);

        // Assert
        await _documentService.Received(1).UpdateDocumentAsync(documentId, updateDto, TestUserId);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteDocument_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var documentId = 1;
        _documentService.DeleteDocumentAsync(documentId, TestUserId).Returns(true);

        // Act
        var result = await _controller.DeleteDocument(documentId);

        // Assert
        await _documentService.Received(1).DeleteDocumentAsync(documentId, TestUserId);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteDocument_ReturnsNotFound_WhenDocumentDoesNotExist()
    {
        // Arrange
        var documentId = 999;
        _documentService.DeleteDocumentAsync(documentId, TestUserId).Returns(false);

        // Act
        var result = await _controller.DeleteDocument(documentId);

        // Assert
        await _documentService.Received(1).DeleteDocumentAsync(documentId, TestUserId);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetDocumentVersions_ReturnsOkWithVersions()
    {
        // Arrange
        var documentId = 1;
        var versions = new List<DocumentVersionResponseDto>
        {
            new DocumentVersionResponseDto
            {
                Id = 1,
                DocumentId = documentId,
                Content = "Version 1",
                VersionNumber = 1,
                UserId = TestUserId,
                CreatedAt = DateTime.UtcNow
            }
        };

        _documentService.GetDocumentVersionsAsync(documentId, TestUserId).Returns(versions);

        // Act
        var result = await _controller.GetDocumentVersions(documentId);

        // Assert
        await _documentService.Received(1).GetDocumentVersionsAsync(documentId, TestUserId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedVersions = Assert.IsAssignableFrom<IEnumerable<DocumentVersionResponseDto>>(okResult.Value);
        Assert.Single(returnedVersions);
    }

    [Fact]
    public async Task RevertToVersion_ReturnsOkWithDocument_WhenSuccessful()
    {
        // Arrange
        var documentId = 1;
        var revertDto = new RevertToVersionDto
        {
            VersionNumber = 1,
            ChangeDescription = "Reverted"
        };

        var revertedDocument = new DocumentResponseDto
        {
            Id = documentId,
            Title = "Test Doc",
            Content = "Reverted Content",
            UserId = TestUserId,
            CurrentVersion = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _documentService.RevertToVersionAsync(documentId, revertDto, TestUserId).Returns(revertedDocument);

        // Act
        var result = await _controller.RevertToVersion(documentId, revertDto);

        // Assert
        await _documentService.Received(1).RevertToVersionAsync(documentId, revertDto, TestUserId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDoc = Assert.IsType<DocumentResponseDto>(okResult.Value);
        Assert.Equal(3, returnedDoc.CurrentVersion);
    }

    [Fact]
    public async Task RevertToVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var documentId = 1;
        var revertDto = new RevertToVersionDto
        {
            VersionNumber = 999
        };

        _documentService.RevertToVersionAsync(documentId, revertDto, TestUserId).Returns((DocumentResponseDto?)null);

        // Act
        var result = await _controller.RevertToVersion(documentId, revertDto);

        // Assert
        await _documentService.Received(1).RevertToVersionAsync(documentId, revertDto, TestUserId);
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
