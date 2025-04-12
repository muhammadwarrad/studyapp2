// Services/IAuthenticationService.cs
using LearningStarter.Entities;

namespace LearningStarter.Services;

public interface IAuthenticationService
{
    User GetLoggedInUser();
}