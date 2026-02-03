using DocRedactor.API.DTOs;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;

namespace DocRedactor.API.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _versionRepository;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        IDocumentRepository documentRepository,
        IDocumentVersionRepository versionRepository,
        ILogger<DocumentService> logger)
    {
        _documentRepository = documentRepository;
        _versionRepository = versionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsByUserAsync(string userId)
    {
        var documents = await _documentRepository.GetAllByUserIdAsync(userId);
        return documents.Select(MapToResponseDto);
    }

    public async Task<DocumentResponseDto?> GetDocumentByIdAsync(int id, string userId)
    {
        var document = await _documentRepository.GetByIdAndUserIdAsync(id, userId);
        return document == null ? null : MapToResponseDto(document);
    }

    public async Task<DocumentResponseDto> CreateDocumentAsync(CreateDocumentDto createDto, string userId)
    {
        var document = new Document
        {
            Title = createDto.Title,
            Content = createDto.Content,
            UserId = userId,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdDocument = await _documentRepository.CreateAsync(document);

        // Create initial version
        var initialVersion = new DocumentVersion
        {
            DocumentId = createdDocument.Id,
            Content = createDto.Content,
            VersionNumber = 1,
            ChangeDescription = "Initial version",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.CreateAsync(initialVersion);

        _logger.LogInformation("Document {DocumentId} created by user {UserId}", createdDocument.Id, userId);

        return MapToResponseDto(createdDocument);
    }

    public async Task<DocumentResponseDto?> UpdateDocumentAsync(int id, UpdateDocumentDto updateDto, string userId)
    {
        var document = await _documentRepository.GetByIdAndUserIdAsync(id, userId);
        if (document == null)
        {
            return null;
        }

        // Update document
        document.Content = updateDto.Content;
        document.CurrentVersion++;

        // Create new version
        var newVersion = new DocumentVersion
        {
            DocumentId = document.Id,
            Content = updateDto.Content,
            VersionNumber = document.CurrentVersion,
            ChangeDescription = updateDto.ChangeDescription ?? "Updated content",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.CreateAsync(newVersion);
        await _documentRepository.UpdateAsync(document);

        _logger.LogInformation("Document {DocumentId} updated to version {Version} by user {UserId}", 
            document.Id, document.CurrentVersion, userId);

        return MapToResponseDto(document);
    }

    public async Task<bool> DeleteDocumentAsync(int id, string userId)
    {
        var document = await _documentRepository.GetByIdAndUserIdAsync(id, userId);
        if (document == null)
        {
            return false;
        }

        await _documentRepository.DeleteAsync(document);
        _logger.LogInformation("Document {DocumentId} deleted by user {UserId}", id, userId);
        return true;
    }

    public async Task<IEnumerable<DocumentVersionResponseDto>> GetDocumentVersionsAsync(int documentId, string userId)
    {
        // First verify user owns the document
        var document = await _documentRepository.GetByIdAndUserIdAsync(documentId, userId);
        if (document == null)
        {
            return Enumerable.Empty<DocumentVersionResponseDto>();
        }

        var versions = await _versionRepository.GetVersionsByDocumentIdAsync(documentId);
        return versions.Select(MapVersionToResponseDto);
    }

    public async Task<DocumentResponseDto?> RevertToVersionAsync(int documentId, RevertToVersionDto revertDto, string userId)
    {
        var document = await _documentRepository.GetByIdAndUserIdAsync(documentId, userId);
        if (document == null)
        {
            return null;
        }

        var versionToRevert = await _versionRepository.GetVersionByDocumentAndVersionNumberAsync(
            documentId, revertDto.VersionNumber);
        
        if (versionToRevert == null)
        {
            return null;
        }

        // Update document with content from the reverted version
        document.Content = versionToRevert.Content;
        document.CurrentVersion++;

        // Create new version with reverted content
        var newVersion = new DocumentVersion
        {
            DocumentId = document.Id,
            Content = versionToRevert.Content,
            VersionNumber = document.CurrentVersion,
            ChangeDescription = revertDto.ChangeDescription ?? $"Reverted to version {revertDto.VersionNumber}",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.CreateAsync(newVersion);
        await _documentRepository.UpdateAsync(document);

        _logger.LogInformation("Document {DocumentId} reverted to version {VersionNumber}, creating new version {NewVersion}", 
            documentId, revertDto.VersionNumber, document.CurrentVersion);

        return MapToResponseDto(document);
    }

    public async Task<DocumentVersionResponseDto?> RateVersionAsync(int documentId, int versionId, RateVersionDto rateDto, string userId)
    {
        // First verify user owns the document
        var document = await _documentRepository.GetByIdAndUserIdAsync(documentId, userId);
        if (document == null)
        {
            return null;
        }

        // Get the version
        var version = await _versionRepository.GetVersionByIdAsync(versionId);
        if (version == null || version.DocumentId != documentId)
        {
            return null;
        }

        // Update the rating
        version.Rating = rateDto.Rating;
        await _versionRepository.UpdateAsync(version);

        _logger.LogInformation("Version {VersionId} of document {DocumentId} rated {Rating} by user {UserId}",
            versionId, documentId, rateDto.Rating, userId);

        return MapVersionToResponseDto(version);
    }

    private static DocumentResponseDto MapToResponseDto(Document document)
    {
        // Find the current version's rating
        var currentVersionRating = document.Versions
            .FirstOrDefault(v => v.VersionNumber == document.CurrentVersion)?.Rating;

        return new DocumentResponseDto
        {
            Id = document.Id,
            Title = document.Title,
            Content = document.Content,
            CurrentVersion = document.CurrentVersion,
            CurrentVersionRating = currentVersionRating,
            UserId = document.UserId,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    private static DocumentVersionResponseDto MapVersionToResponseDto(DocumentVersion version)
    {
        return new DocumentVersionResponseDto
        {
            Id = version.Id,
            DocumentId = version.DocumentId,
            Content = version.Content,
            VersionNumber = version.VersionNumber,
            ChangeDescription = version.ChangeDescription,
            Rating = version.Rating,
            UserId = version.UserId,
            CreatedAt = version.CreatedAt
        };
    }
}
