using DocRedactor.API.Models;

namespace DocRedactor.API.Interfaces;

public interface IDocumentVersionRepository
{
    Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId);
    Task<DocumentVersion?> GetVersionByIdAsync(int id);
    Task<DocumentVersion?> GetVersionByDocumentAndVersionNumberAsync(int documentId, int versionNumber);
    Task<DocumentVersion> CreateAsync(DocumentVersion version);
    Task UpdateAsync(DocumentVersion version);
    Task<int> GetNextVersionNumberAsync(int documentId);
}
