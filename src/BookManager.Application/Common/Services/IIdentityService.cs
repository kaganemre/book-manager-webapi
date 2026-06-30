using BookManager.Application.Common.Models;

namespace BookManager.Application.Common.Services;

public interface IIdentityService
{
    Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password);
}