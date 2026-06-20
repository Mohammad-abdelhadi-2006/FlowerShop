using FlowerShop.API.DTOs;

namespace FlowerShop.API.Services;

public enum UpdateUserResult
{
    Success,
    NotFound,
    EmailConflict
}

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);

    // Returns null when the email is already taken (conflict).
    Task<UserDto?> CreateAsync(CreateUserDto dto);
    Task<UpdateUserResult> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}
