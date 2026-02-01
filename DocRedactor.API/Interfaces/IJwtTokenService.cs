using DocRedactor.API.Models;

namespace DocRedactor.API.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}
