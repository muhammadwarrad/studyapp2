using System.Collections.Generic;
using System.Linq;
using LearningStarter.Common;
using LearningStarter.Data;
using LearningStarter.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningStarter.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;

    public UsersController(DataContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response
        {
            Data = _context
                .Users
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
                .ToList(),
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(
        [FromRoute] int id)
    {
        var response = new Response();

        var user = _context.Users.Include(user => user.FlashcardSets)
            .ThenInclude(userFlashcardSet => userFlashcardSet.FlashcardSet).Include(user => user.FlashcardSets)
            .ThenInclude(userFlashcardSet => userFlashcardSet.Flashcards)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            response.AddError("id", "There was a problem finding the user.");
            return NotFound(response);
        }

        var userGetDto = new UserGetDto
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
                Flashcards = y.Flashcards.Select(fc => new FlashcardDto
                {
                    Id = fc.Id,
                    Front = fc.Front,
                    Back = fc.Back,
                }).ToList(),
                    
            }).ToList(),
        };

        response.Data = userGetDto;

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create(
        [FromBody] UserCreateDto userCreateDto)
    {
        var response = new Response();

        if (string.IsNullOrEmpty(userCreateDto.FirstName))
        {
            response.AddError("firstName", "First name cannot be empty.");
        }

        if (string.IsNullOrEmpty(userCreateDto.LastName))
        {
            response.AddError("lastName", "Last name cannot be empty.");
        }
        if (string.IsNullOrEmpty(userCreateDto.Email))
        {
            response.AddError("email", "email cannot be empty.");
        }

        if (string.IsNullOrEmpty(userCreateDto.UserName))
        {
            response.AddError("userName", "User name cannot be empty.");
        }

        if (string.IsNullOrEmpty(userCreateDto.Password))
        {
            response.AddError("password", "Password cannot be empty.");
        }
        

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        var userToCreate = new User
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            Email = userCreateDto.Email,
            UserName = userCreateDto.UserName,
        };

        _userManager.CreateAsync(userToCreate, userCreateDto.Password).Wait();
        _userManager.AddToRoleAsync(userToCreate, "Admin").Wait();
        _context.SaveChanges();

        var userGetDto = new UserGetDto
        {
            Id = userToCreate.Id,
            FirstName = userToCreate.FirstName,
            LastName = userToCreate.LastName,
            Email = userToCreate.Email,
            UserName = userToCreate.UserName
        };

        response.Data = userGetDto;

        return Created("", response);
    }
    [HttpPost("{userId:int}/flashcardSet")]
    public IActionResult AddFlashCardToSet(int userId, int flashcardSetId)
    {
        var response = new Response();
        
        var user = _context.Set<User>()
            .FirstOrDefault(x => x.Id == userId);
        
        var flashcardSet = _context.Set<FlashcardSet>()
            .FirstOrDefault(x => x.Id == flashcardSetId);

        if (user == null)
        {
            response.AddError("user", "User Not Found.");
        }
        if (flashcardSet == null)
        {
            response.AddError("flashcardSet", "Flashcard Set Not Found.");
        }

        if (user != null)
        {
            var userFlashcardSets = new UserFlashcardSet
            {
                UserId = user.Id,
                FlashcardSet = flashcardSet,
            };
            _context.Set<UserFlashcardSet>().Add(userFlashcardSets);
        }

        _context.SaveChanges();
        
        
        //this takes all info updated and then uses it to provided updated values (I think)
        var updatedUser = _context.Users
            .Include(u => u.FlashcardSets)
            .ThenInclude(ufs => ufs.FlashcardSet).ThenInclude(flashcardSet => flashcardSet.Flashcards)
            .Include(user => user.FlashcardSets).ThenInclude(userFlashcardSet => userFlashcardSet.Flashcards)
            .FirstOrDefault(x => x.Id == userId);

        if (updatedUser != null)
            response.Data = new UserGetDto
            {
                Id = updatedUser.Id,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email,
                UserName = updatedUser.UserName,
                FlashcardSets = updatedUser.FlashcardSets.Select(x => new FlashcardSetForUserDto
                {
                    Id = x.FlashcardSet.Id,
                    Title = x.FlashcardSet.Title,
                    Description = x.FlashcardSet.Description,
                    Flashcards = x.Flashcards.Select(y => new FlashcardDto
                    {
                        Id = y.Id,
                        Front = y.Front,
                        Back = y.Back,
                    }).ToList(),
                }).ToList(),
    };
        return Ok(response);


    }

    [HttpPut("{id}")]
    public IActionResult Edit(
        [FromRoute] int id, 
        [FromBody] UserUpdateDto userUpdateDto)
    {
        var response = new Response();

        if (userUpdateDto == null)
        {
            response.AddError("id", "There was a problem editing the user.");
            return NotFound(response);
        }
        
        var userToEdit = _context.Users.FirstOrDefault(x => x.Id == id);

        if (userToEdit == null)
        {
            response.AddError("id", "Could not find user to edit.");
            return NotFound(response);
        }

        if (string.IsNullOrEmpty(userUpdateDto.FirstName))
        {
            response.AddError("firstName", "First name cannot be empty.");
        }

        if (string.IsNullOrEmpty(userUpdateDto.LastName))
        {
            response.AddError("lastName", "Last name cannot be empty.");
        }
        if (string.IsNullOrEmpty(userUpdateDto.Email))
        {
            response.AddError("email", "email cannot be empty.");
        }
        if (string.IsNullOrEmpty(userUpdateDto.UserName))
        {
            response.AddError("userName", "User name cannot be empty.");
        }

        if (string.IsNullOrEmpty(userUpdateDto.Password))
        {
            response.AddError("password", "Password cannot be empty.");
        }

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        userToEdit.FirstName = userUpdateDto.FirstName;
        userToEdit.LastName = userUpdateDto.LastName;
        userToEdit.Email = userUpdateDto.Email;
        userToEdit.UserName = userUpdateDto.UserName;

        _context.SaveChanges();
        
         var userGetDto = new UserGetDto
        {
            Id = userToEdit.Id,
            FirstName = userToEdit.FirstName,
            LastName = userToEdit.LastName,
            Email = userToEdit.Email,
            UserName = userToEdit.UserName,
        };

        response.Data = userGetDto;
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();

        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            response.AddError("id", "There was a problem deleting the user.");
            return NotFound(response);
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Ok(response);
    }
    
}
