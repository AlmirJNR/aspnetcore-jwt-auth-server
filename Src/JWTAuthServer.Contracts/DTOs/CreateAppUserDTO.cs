using Data.Models;

namespace Contracts.DTOs;

public class CreateAppUserDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public AppUser ToModel() => new AppUser { Email = Email, Password = Password };
}