using Microsoft.AspNetCore.Identity;

namespace BookManager.Application.Common.Services;

public interface IJwtTokenService
{
    Task<string> GenerateToken(IdentityUser user);
}