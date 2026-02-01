using Microsoft.AspNetCore.Identity;

namespace DocRedactor.API.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<DocumentVersion> DocumentVersions { get; set; } = new List<DocumentVersion>();
}
