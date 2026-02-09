using DocRedactor.API.Models;

namespace DocRedactor.API.Interfaces;

public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetAllByUserIdAsync(string userId);
    Task<Document?> GetByIdAsync(int id);
    Task<Document?> GetByIdAndUserIdAsync(int id, string userId);
    Task<Document> CreateAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Document document);
}
