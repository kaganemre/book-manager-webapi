using BookManager.Application.Common.Models;

namespace BookManager.Application.Common.Services;

public interface IIdentityService
{
    Task<UserInfo?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(UserInfo user, string password);
}