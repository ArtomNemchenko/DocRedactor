using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DocRedactor.API.Data;
using DocRedactor.API.DTOs;
using DocRedactor.API.Models;

namespace DocRedactor.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentsController> _logger;
    
    public DocumentsController(ApplicationDbContext context, ILogger<DocumentsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentResponseDto>>> GetDocuments()
    {
        try
        {
            var userId = GetUserId();
            var documents = await _context.Documents
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DocumentResponseDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Content = d.Content,
                    UserId = d.UserId,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
                
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents");
            return StatusCode(500, "An error occurred while retrieving documents");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentResponseDto>> GetDocument(int id)
    {
        try
        {
            var userId = GetUserId();
            var document = await _context.Documents
                .Where(d => d.Id == id && d.UserId == userId)
                .Select(d => new DocumentResponseDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Content = d.Content,
                    UserId = d.UserId,
                    CreatedAt = d.CreatedAt
                })
                .FirstOrDefaultAsync();
                
            if (document == null)
            {
                return NotFound();
            }
            
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {DocumentId}", id);
            return StatusCode(500, "An error occurred while retrieving the document");
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<DocumentResponseDto>> CreateDocument(CreateDocumentDto createDto)
    {
        try
        {
            var userId = GetUserId();
            var document = new Document
            {
                Title = createDto.Title,
                Content = createDto.Content,
                UserId = userId
            };
            
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Document {DocumentId} created by user {UserId}", document.Id, userId);
            
            var responseDto = new DocumentResponseDto
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                UserId = document.UserId,
                CreatedAt = document.CreatedAt
            };
            
            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document");
            return StatusCode(500, "An error occurred while creating the document");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        try
        {
            var userId = GetUserId();
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
                
            if (document == null)
            {
                return NotFound();
            }
            
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Document {DocumentId} deleted by user {UserId}", id, userId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return StatusCode(500, "An error occurred while deleting the document");
        }
    }
}
