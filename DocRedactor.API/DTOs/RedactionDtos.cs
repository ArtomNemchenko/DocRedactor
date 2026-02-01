using System.ComponentModel.DataAnnotations;

namespace DocRedactor.API.DTOs;

public class CreateRedactionDto
{
    [Required]
    public int DocumentId { get; set; }
    
    [Required]
    public int StartPosition { get; set; }
    
    [Required]
    public int EndPosition { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
}

public class RedactionResponseDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
