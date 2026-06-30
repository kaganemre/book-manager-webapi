using BookManager.Application.Common.Models;
using BookManager.Application.Common.Services;
using Microsoft.AspNetCore.Identity;

namespace BookManager.Infrastructure.Identity;

public sealed class IdentityService(UserManager<IdentityUser> userManager) : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthenticatedUser(user.Id, user.Email!, user.UserName!, roles);
    }
}