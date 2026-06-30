using BookManager.Application.Common.Models;
using FluentResults;

namespace BookManager.Application.Common.Services;

public interface IIdentityService
{
    Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password);
    Task<Result> CreateAsync(CreateUserRequest createUserRequest);
}