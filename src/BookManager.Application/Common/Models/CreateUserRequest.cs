namespace BookManager.Application.Common.Models;

public sealed record CreateUserRequest(string UserName, string Email, string Password);