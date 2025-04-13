// Services/AuthenticationService.cs
using System.Linq;
using System.Security.Claims;
using StudyApp.Data;
using StudyApp.Entities;
using Microsoft.AspNetCore.Http;

namespace StudyApp.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public User GetLoggedInUser()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return null;

        return _context.Users.FirstOrDefault(u => u.Id == int.Parse(userIdClaim));
    }
}