using System.Collections.Generic;
using System.Threading.Tasks;
using StudyApp.Entities;

namespace StudyApp.Services;

public interface IUserService
{
    Task<List<UserGetDto>> GetAllAsync();
    Task<UserGetDto> GetByIdAsync(int id);
    Task<UserGetDto> CreateAsync(UserCreateDto createDto);
    Task<UserGetDto> UpdateAsync(int id, UserUpdateDto updateDto);
    Task<bool> DeleteAsync(int id);
}