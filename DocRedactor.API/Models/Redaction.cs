using System.ComponentModel.DataAnnotations;

namespace DocRedactor.API.Models;

public class Redaction
{
    public int Id { get; set; }
    
    public int DocumentId { get; set; }
    
    public Document? Document { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public ApplicationUser? User { get; set; }
    
    [Required]
    public int StartPosition { get; set; }
    
    [Required]
    public int EndPosition { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
