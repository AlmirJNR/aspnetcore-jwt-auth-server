using Data.Models;

namespace Contracts.DTOs;

public readonly record struct GetAppUserDTO()
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

    public AppUser ToModel() => new AppUser { Email = Email, Password = Password };
}