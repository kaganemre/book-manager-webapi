using BookManager.Application.Common.Models;

namespace BookManager.Application.Common.Services;

public interface IJwtTokenService
{
    string GenerateToken(AuthenticatedUser user);
}