// Services/IAuthenticationService.cs
using StudyApp.Entities;

namespace StudyApp.Services;

public interface IAuthenticationService
{
    User GetLoggedInUser();
}