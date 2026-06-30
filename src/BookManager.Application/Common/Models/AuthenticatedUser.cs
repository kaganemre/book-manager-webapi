namespace BookManager.Application.Common.Models;

public sealed record AuthenticatedUser(
    string Id, 
    string Email, 
    string UserName, 
    IList<string> Roles);