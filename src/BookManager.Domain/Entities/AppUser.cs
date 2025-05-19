using Microsoft.AspNetCore.Identity;

namespace BookManager.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}