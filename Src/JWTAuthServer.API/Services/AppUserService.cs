using API.Repositories;
using Contracts.DTOs;
using Data.Models;

namespace API.Services;

public class AppUserService
{
    private readonly AppUserRepository _appUserRepository;

    public AppUserService(AppUserRepository appUserRepository)
    {
        _appUserRepository = appUserRepository;
    }

    public Task<(AppUser?, int)> AddNew(CreateAppUserDTO createAppUserDto)
        => _appUserRepository.AddNew(createAppUserDto.ToModel());

    public Task<AppUser?> GetUserById(Guid userId) => _appUserRepository.GetUserById(userId);
    
    public Task<AppUser?> GetUserWithPassword(GetAppUserDTO getAppUserDto)
        => _appUserRepository.GetUserWithPassword(getAppUserDto.ToModel());
}