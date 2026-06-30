using BookManager.Application.Common.Models;

namespace BookManager.Application.Common.Services;

public interface IJwtTokenService
{
    Task<string> GenerateToken(AuthenticatedUser user);
}