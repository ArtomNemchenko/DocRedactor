using System.ComponentModel.DataAnnotations;

namespace DocRedactor.API.Models;

public class Document
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public int CurrentVersion { get; set; } = 1;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public ApplicationUser? User { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
}
