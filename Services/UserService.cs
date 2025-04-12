using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningStarter.Data;
using LearningStarter.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningStarter.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;

    public UserService(DataContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<UserGetDto>> GetAllAsync()
    {
        return await _context.Users
            .Select(x => new UserGetDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                UserName = x.UserName,
                FlashcardSets = x.FlashcardSets.Select(y => new FlashcardSetForUserDto
                {
                    Id = y.FlashcardSet.Id,
                    Title = y.FlashcardSet.Title,
                    Description = y.FlashcardSet.Description,
                    Flashcards = y.FlashcardSet.Flashcards.Select(fc => new FlashcardDto
                    {
                        Id = fc.Id,
                        Front = fc.Front,
                        Back = fc.Back,
                    }).ToList(),
                }).ToList(),
            })
            .ToListAsync();
    }

    public async Task<UserGetDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.FlashcardSets)
            .ThenInclude(ufs => ufs.FlashcardSet)
            .ThenInclude(fs => fs.Flashcards)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new UserGetDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            FlashcardSets = user.FlashcardSets.Select(y => new FlashcardSetForUserDto
            {
                Id = y.FlashcardSet.Id,
                Title = y.FlashcardSet.Title,
                Description = y.FlashcardSet.Description,
                Flashcards = y.FlashcardSet.Flashcards.Select(fc => new FlashcardDto
                {
                    Id = fc.Id,
                    Front = fc.Front,
                    Back = fc.Back,
                }).ToList(),
            }).ToList(),
        };
    }

    public async Task<UserGetDto> CreateAsync(UserCreateDto createDto)
    {
        var user = new User
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            UserName = createDto.UserName,
        };

        var result = await _userManager.CreateAsync(user, createDto.Password);
        if (!result.Succeeded) return null;

        await _userManager.AddToRoleAsync(user, "Admin");

        return new UserGetDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
        };
    }

    public async Task<UserGetDto> UpdateAsync(int id, UserUpdateDto updateDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return null;

        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.Email = updateDto.Email;
        user.UserName = updateDto.UserName;

        if (!string.IsNullOrEmpty(updateDto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
        }

        await _context.SaveChangesAsync();

        return new UserGetDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}