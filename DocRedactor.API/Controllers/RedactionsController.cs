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
public class RedactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RedactionsController> _logger;
    
    public RedactionsController(ApplicationDbContext context, ILogger<RedactionsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found");
    }
    
    [HttpGet("document/{documentId}")]
    public async Task<ActionResult<IEnumerable<RedactionResponseDto>>> GetRedactionsByDocument(int documentId)
    {
        try
        {
            var userId = GetUserId();
            
            // Verify user owns the document
            var documentExists = await _context.Documents
                .AnyAsync(d => d.Id == documentId && d.UserId == userId);
                
            if (!documentExists)
            {
                return NotFound("Document not found or access denied");
            }
            
            var redactions = await _context.Redactions
                .Where(r => r.DocumentId == documentId)
                .OrderBy(r => r.StartPosition)
                .Select(r => new RedactionResponseDto
                {
                    Id = r.Id,
                    DocumentId = r.DocumentId,
                    UserId = r.UserId,
                    StartPosition = r.StartPosition,
                    EndPosition = r.EndPosition,
                    Reason = r.Reason,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
                
            return Ok(redactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving redactions for document {DocumentId}", documentId);
            return StatusCode(500, "An error occurred while retrieving redactions");
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<RedactionResponseDto>> CreateRedaction(CreateRedactionDto createDto)
    {
        try
        {
            var userId = GetUserId();
            
            // Verify user owns the document
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == createDto.DocumentId && d.UserId == userId);
                
            if (document == null)
            {
                return NotFound("Document not found or access denied");
            }
            
            // Validate positions
            if (createDto.StartPosition < 0 || createDto.EndPosition > document.Content.Length 
                || createDto.StartPosition >= createDto.EndPosition)
            {
                return BadRequest("Invalid redaction positions");
            }
            
            var redaction = new Redaction
            {
                DocumentId = createDto.DocumentId,
                UserId = userId,
                StartPosition = createDto.StartPosition,
                EndPosition = createDto.EndPosition,
                Reason = createDto.Reason
            };
            
            _context.Redactions.Add(redaction);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Redaction {RedactionId} created for document {DocumentId} by user {UserId}", 
                redaction.Id, createDto.DocumentId, userId);
            
            var responseDto = new RedactionResponseDto
            {
                Id = redaction.Id,
                DocumentId = redaction.DocumentId,
                UserId = redaction.UserId,
                StartPosition = redaction.StartPosition,
                EndPosition = redaction.EndPosition,
                Reason = redaction.Reason,
                CreatedAt = redaction.CreatedAt
            };
            
            return CreatedAtAction(nameof(GetRedactionsByDocument), 
                new { documentId = redaction.DocumentId }, responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating redaction");
            return StatusCode(500, "An error occurred while creating the redaction");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRedaction(int id)
    {
        try
        {
            var userId = GetUserId();
            
            // Verify user owns the redaction
            var redaction = await _context.Redactions
                .Include(r => r.Document)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (redaction == null)
            {
                return NotFound();
            }
            
            // Check if user owns the document
            if (redaction.Document?.UserId != userId)
            {
                return Forbid();
            }
            
            _context.Redactions.Remove(redaction);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Redaction {RedactionId} deleted by user {UserId}", id, userId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting redaction {RedactionId}", id);
            return StatusCode(500, "An error occurred while deleting the redaction");
        }
    }
}
