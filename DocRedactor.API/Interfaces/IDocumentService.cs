using DocRedactor.API.DTOs;
using DocRedactor.API.Models;

namespace DocRedactor.API.Interfaces;

public interface IDocumentService
{
    Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsByUserAsync(string userId);
    Task<DocumentResponseDto?> GetDocumentByIdAsync(int id, string userId);
    Task<DocumentResponseDto> CreateDocumentAsync(CreateDocumentDto createDto, string userId);
    Task<DocumentResponseDto?> UpdateDocumentAsync(int id, UpdateDocumentDto updateDto, string userId);
    Task<bool> DeleteDocumentAsync(int id, string userId);
    Task<IEnumerable<DocumentVersionResponseDto>> GetDocumentVersionsAsync(int documentId, string userId);
    Task<DocumentResponseDto?> RevertToVersionAsync(int documentId, RevertToVersionDto revertDto, string userId);
    Task<DocumentVersionResponseDto?> RateVersionAsync(int documentId, int versionId, RateVersionDto rateDto, string userId);
}
