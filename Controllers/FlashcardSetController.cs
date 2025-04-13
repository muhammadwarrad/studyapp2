using System.Linq;
using StudyApp.Common;
using StudyApp.Data;
using StudyApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace StudyApp.Controllers;




[ApiController]
[Route("/api/flashcardSet")]
public class FlashcardSetController : ControllerBase
{
    private readonly DataContext _dataContext;

    public FlashcardSetController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response();
        
        var data = _dataContext.Set<FlashcardSet>().Select(flashcardSet => new FlashcardSetGetDto
        {
            Id = flashcardSet.Id,
            Title = flashcardSet.Title,
            Description = flashcardSet.Description,
            Flashcards = flashcardSet.Flashcards.Select(fc => new FlashcardGetDto
            {
                Id = fc.Id,
                Front = fc.Front,
                Back = fc.Back,
            }).ToList(),
            Users = flashcardSet.Users.Select(x => new UserInfoForSet
            {
                Id = x.User.Id,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                UserName = x.User.UserName,
            }).ToList(),
        }).ToList();
        response.Data = data;
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var response = new Response();

        if (id <= 0)
        {
            return BadRequest("Invalid FlashCardSet ID, must be greater than 0.");
        }

        
        var flashcardSet = _dataContext.Set<FlashcardSet>()
            .Where(x => x.Id == id)
            .Select(x => new FlashcardSetGetDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Flashcards = x.Flashcards.Select(fc => new FlashcardGetDto
                {
                    Id = fc.Id,
                    Front = fc.Front,
                    Back = fc.Back,
                }).ToList(),
            }).FirstOrDefault();

        if (flashcardSet == null)
        {
            response.AddError("id", "Flashcard set not found.");
            return NotFound(response);
        }

        response.Data = flashcardSet;
        return Ok(response);
    }

    [HttpPost("{id}/flashcard")]
    public IActionResult AddFlashCardToSet(int id, [FromBody] FlashcardCreateDto flashcardCreateDto)
    {
        var response = new Response();

        var flashcardSet = _dataContext.Set<FlashcardSet>().Include(fs => fs.Flashcards)
            .FirstOrDefault(fs => fs.Id == id);
        
        
        var flashcardToCreate = new FlashcardDto
        {
            Front = flashcardCreateDto.Front,
            Back = flashcardCreateDto.Back,
        };
        if (flashcardSet != null) flashcardSet.Flashcards.Add(flashcardToCreate);

        _dataContext.Set<FlashcardDto>().Add(flashcardToCreate);
        _dataContext.SaveChanges();

        var flashcardToReturn = new FlashcardGetDto
        {
            Id = flashcardToCreate.Id,
            Front = flashcardToCreate.Front,
            Back = flashcardToCreate.Back,
        };
        response.Data = flashcardToReturn;
        return Created("", response);
    }
    

    [HttpPost]
    public IActionResult Create([FromBody] FlashcardSetCreateDto createDto)
    {
        var response = new Response();

        if (string.IsNullOrEmpty(createDto.Title))
        {
            response.AddError("title", "Title must not be empty");
        }

        if (string.IsNullOrEmpty(createDto.Description))
        {
            response.AddError("description", "Description must not be empty");
        }

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        var flashcardSetToCreate = new FlashcardSet
        {
            Title = createDto.Title,
            Description = createDto.Description,
            //FlashcardId = createDto.FlashcardId,
            Flashcards = createDto.Flashcards.Select(x => new FlashcardDto
            {
                Front = x.Front,
                Back = x.Back,
            }).ToList(),
            
        };
        _dataContext.Set<FlashcardSet>().Add(flashcardSetToCreate);
        _dataContext.SaveChanges();

        var flashcardSetToReturn = new FlashcardSetGetDto
        {
            Id = flashcardSetToCreate.Id,
            Title = flashcardSetToCreate.Title,
            Description = flashcardSetToCreate.Description,
            Flashcards = flashcardSetToCreate.Flashcards.Select(fc => new FlashcardGetDto
            {
                Id = fc.Id,
                Front = fc.Front,
                Back = fc.Back,
            }).ToList(),
            
        };
        response.Data = flashcardSetToReturn;
        return Created("", response);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromBody] FlashcardSetUpdateDto updateDto, [FromRoute] int id)
    {
        var response = new Response();
        
        var flashcardSetToUpdate = _dataContext.Set<FlashcardSet>().FirstOrDefault(flashcardSet => flashcardSet.Id == id);

        if (string.IsNullOrEmpty(updateDto.Title))
        {
            response.AddError("title", "Title cannot be empty.");
        }

        if (string.IsNullOrEmpty(updateDto.Description))
        {
            response.AddError("description", "Description cannot be empty.");
        }

        if (flashcardSetToUpdate == null)
        {
            response.AddError("id", "Flashcard Set Not Found.");
        }
        if (response.HasErrors)
        {
            return BadRequest(response);
        }


        flashcardSetToUpdate.Title = updateDto.Title;
        flashcardSetToUpdate.Description = updateDto.Description;
        flashcardSetToUpdate.Flashcards = updateDto.Flashcards.Select(x => new FlashcardDto
        {
            Front = x.Front,
            Back = x.Back,
        }).ToList();
        
        _dataContext.SaveChanges();

        var flashcardSetToReturn = new FlashcardSetGetDto()
        {
            Id = flashcardSetToUpdate.Id,
            Title = flashcardSetToUpdate.Title,
            Description = flashcardSetToUpdate.Description,
            Flashcards = flashcardSetToUpdate.Flashcards.Select(fc => new FlashcardGetDto
            {
                Id = fc.Id,
                Front = fc.Front,
                Back = fc.Back,
            }).ToList(),
        };
        
        response.Data = flashcardSetToReturn;
        return Ok(response);
    }
    [HttpDelete("{id}/flashcard/{flashcardId:int}")]
    public IActionResult DeleteFlashcard(int id)
    {
        var response = new Response();
        var flashcardSetToDelete = _dataContext.Set<FlashcardSet>().Include(fs => fs.Flashcards).FirstOrDefault(fs => fs.Id == id);
    
        var flashcard =_dataContext.Set<FlashcardDto>().FirstOrDefault(f => f.Id == id);

        if (flashcardSetToDelete == null)
        {
            response.AddError("id", "Flashcard Set Not Found.");
        }

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        if (flashcardSetToDelete != null)
        {
            if (flashcard != null) _dataContext.Set<FlashcardDto>().Remove(flashcard);
        }

        _dataContext.SaveChanges();

        response.Data = true;
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();
        
        var flashcardSetToDelete = _dataContext.Set<FlashcardSet>().FirstOrDefault(flashcardSet => flashcardSet.Id == id);
        if (flashcardSetToDelete == null)
        {
            response.AddError("id", "Flashcard Set Not Found.");
        }

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        _dataContext.Set<FlashcardSet>().Remove(flashcardSetToDelete);
        _dataContext.SaveChanges();

        response.Data = true;
        return Ok(response);
    }
}