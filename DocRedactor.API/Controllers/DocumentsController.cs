using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DocRedactor.API.DTOs;
using DocRedactor.API.Interfaces;

namespace DocRedactor.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;
    
    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
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
            var documents = await _documentService.GetAllDocumentsByUserAsync(userId);
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
            var document = await _documentService.GetDocumentByIdAsync(id, userId);
                
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
            var document = await _documentService.CreateDocumentAsync(createDto, userId);
            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document");
            return StatusCode(500, "An error occurred while creating the document");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DocumentResponseDto>> UpdateDocument(int id, UpdateDocumentDto updateDto)
    {
        try
        {
            var userId = GetUserId();
            var document = await _documentService.UpdateDocumentAsync(id, updateDto, userId);
                
            if (document == null)
            {
                return NotFound();
            }
            
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document {DocumentId}", id);
            return StatusCode(500, "An error occurred while updating the document");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        try
        {
            var userId = GetUserId();
            var result = await _documentService.DeleteDocumentAsync(id, userId);
                
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return StatusCode(500, "An error occurred while deleting the document");
        }
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult<IEnumerable<DocumentVersionResponseDto>>> GetDocumentVersions(int id)
    {
        try
        {
            var userId = GetUserId();
            var versions = await _documentService.GetDocumentVersionsAsync(id, userId);
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving versions for document {DocumentId}", id);
            return StatusCode(500, "An error occurred while retrieving document versions");
        }
    }

    [HttpPost("{id}/revert")]
    public async Task<ActionResult<DocumentResponseDto>> RevertToVersion(int id, RevertToVersionDto revertDto)
    {
        try
        {
            var userId = GetUserId();
            var document = await _documentService.RevertToVersionAsync(id, revertDto, userId);
                
            if (document == null)
            {
                return NotFound();
            }
            
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting document {DocumentId} to version {VersionNumber}", 
                id, revertDto.VersionNumber);
            return StatusCode(500, "An error occurred while reverting the document");
        }
    }

    [HttpPut("{id}/versions/{versionId}/rate")]
    public async Task<ActionResult<DocumentVersionResponseDto>> RateVersion(int id, int versionId, RateVersionDto rateDto)
    {
        try
        {
            var userId = GetUserId();
            var version = await _documentService.RateVersionAsync(id, versionId, rateDto, userId);
                
            if (version == null)
            {
                return NotFound();
            }
            
            return Ok(version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating version {VersionId} of document {DocumentId}", versionId, id);
            return StatusCode(500, "An error occurred while rating the version");
        }
    }
}
