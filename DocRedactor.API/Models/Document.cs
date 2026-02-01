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
    public string UserId { get; set; } = string.Empty;
    
    public ApplicationUser? User { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public ICollection<Redaction> Redactions { get; set; } = new List<Redaction>();
}
