using System.ComponentModel.DataAnnotations;

namespace DocRedactor.API.Models;

public class DocumentVersion
{
    public int Id { get; set; }
    
    public int DocumentId { get; set; }
    
    public Document? Document { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public int VersionNumber { get; set; }
    
    [MaxLength(500)]
    public string? ChangeDescription { get; set; }
    
    [Range(1, 5)]
    public int? Rating { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public ApplicationUser? User { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
