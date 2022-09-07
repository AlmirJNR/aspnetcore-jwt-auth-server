using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class AppUserRepository
{
    private readonly JWTContext _jwtContext;
    private readonly DbSet<AppUser> _appUsersEntity;

    public AppUserRepository(JWTContext jwtContext)
    {
        _jwtContext = jwtContext;
        _appUsersEntity = jwtContext.AppUsers;
    }

    public async Task<(AppUser?, int)> AddNew(AppUser appUser)
    {
        try
        {
            var entityEntry = await _appUsersEntity.AddAsync(appUser);
            var response = await _jwtContext.SaveChangesAsync();

            return response != 1
                ? (null, StatusCodes.Status400BadRequest)
                : (entityEntry.Entity, StatusCodes.Status201Created);
        }
        catch (Exception _)
        {
            return (null, StatusCodes.Status409Conflict);
        }
    }

    public Task<AppUser?> GetUserById(Guid userId)
        => _appUsersEntity.FirstOrDefaultAsync(source => source.Id == userId);

    public Task<AppUser?> GetUserWithPassword(AppUser appUser)
        => _appUsersEntity.FirstOrDefaultAsync(source =>
            source.Email == appUser.Email && source.Password == appUser.Password);
}