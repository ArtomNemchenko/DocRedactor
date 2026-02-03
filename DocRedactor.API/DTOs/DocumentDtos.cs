using System.ComponentModel.DataAnnotations;

namespace DocRedactor.API.DTOs;

public class CreateDocumentDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
}

public class UpdateDocumentDto
{
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ChangeDescription { get; set; }
}

public class DocumentResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CurrentVersion { get; set; }
    public int? CurrentVersionRating { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DocumentVersionResponseDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public string? ChangeDescription { get; set; }
    public int? Rating { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RateVersionDto
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}

public class RevertToVersionDto
{
    [Required]
    public int VersionNumber { get; set; }
    
    [MaxLength(500)]
    public string? ChangeDescription { get; set; }
}
