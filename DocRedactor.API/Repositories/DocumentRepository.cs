using Microsoft.EntityFrameworkCore;
using DocRedactor.API.Data;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;

namespace DocRedactor.API.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Documents
            .Include(d => d.Versions)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document?> GetByIdAndUserIdAsync(int id, string userId)
    {
        return await _context.Documents
            .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber))
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
    }

    public async Task<Document> CreateAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task UpdateAsync(Document document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Document document)
    {
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }
}
