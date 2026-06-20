using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        // Reject duplicate emails before hitting the unique index.
        var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailTaken)
            return null;

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UpdateUserResult> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return UpdateUserResult.NotFound;

        // Block changing to an email another user already owns.
        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email == dto.Email && u.Id != id);
        if (emailTaken)
            return UpdateUserResult.EmailConflict;

        user.Name = dto.Name;
        user.Email = dto.Email;

        await _context.SaveChangesAsync();
        return UpdateUserResult.Success;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Never expose PasswordHash through the DTO.
    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    };
}
