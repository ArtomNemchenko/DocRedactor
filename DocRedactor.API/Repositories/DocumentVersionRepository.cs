using Microsoft.EntityFrameworkCore;
using DocRedactor.API.Data;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;

namespace DocRedactor.API.Repositories;

public class DocumentVersionRepository : IDocumentVersionRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentVersionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId)
    {
        return await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<DocumentVersion?> GetVersionByIdAsync(int id)
    {
        return await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<DocumentVersion?> GetVersionByDocumentAndVersionNumberAsync(int documentId, int versionNumber)
    {
        return await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == versionNumber);
    }

    public async Task<DocumentVersion> CreateAsync(DocumentVersion version)
    {
        _context.DocumentVersions.Add(version);
        await _context.SaveChangesAsync();
        return version;
    }

    public async Task UpdateAsync(DocumentVersion version)
    {
        _context.DocumentVersions.Update(version);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetNextVersionNumberAsync(int documentId)
    {
        var maxVersion = await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .MaxAsync(v => (int?)v.VersionNumber);
        
        return (maxVersion ?? 0) + 1;
    }
}
