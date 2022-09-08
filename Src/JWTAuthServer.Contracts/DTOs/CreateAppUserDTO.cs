using Data.Models;

namespace Contracts.DTOs;

public readonly record struct CreateAppUserDTO()
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

    public AppUser ToModel() => new AppUser { Email = Email, Password = Password };
}