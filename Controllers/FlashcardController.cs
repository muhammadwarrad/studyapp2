// using System.Linq;
// using LearningStarter.Common;
// using LearningStarter.Data;
// using LearningStarter.Entities;
// using Microsoft.AspNetCore.Mvc;
//
// namespace LearningStarter.Controllers;
//
//
//
//
// [ApiController]
// [Route("/api/flashcard")]
// public class FlashcardController : ControllerBase
// {
//     private readonly DataContext _dataContext;
//
//     public FlashcardController(DataContext dataContext)
//     {
//         _dataContext = dataContext;
//     }
//
//     [HttpGet]
//     public IActionResult GetAll()
//     {
//         var response = new Response();
//         
//         var data = _dataContext.Set<Flashcard>().Select(flashcard => new FlashcardGetDto
//         {
//             Id = flashcard.Id,
//             Term = flashcard.Term,
//             Definition = flashcard.Definition,
//         }).ToList();
//         response.Data = data;
//         return Ok(response);
//     }
//
//     [HttpGet("{id}")]
//     public IActionResult GetById(int id)
//     {
//         var response = new Response();
//
//         if (id <= 0)
//         {
//             return BadRequest("Invalid FlashCard ID, must be greater than 0.");
//         }
//
//         var data = _dataContext.Set<Flashcard>().Select(flashcard => new FlashcardGetDto
//         {
//             Id = flashcard.Id,
//             Term = flashcard.Term,
//             Definition = flashcard.Definition,
//
//         }).ToList();
//         response.Data = data;
//         return Ok(response);
//     }
//
//     [HttpPost]
//     public IActionResult Create([FromBody] FlashcardCreateDto createDto)
//     {
//         var response = new Response();
//
//         if (string.IsNullOrEmpty(createDto.Term))
//         {
//             response.AddError("term", "Term must not be empty");
//         }
//
//         if (string.IsNullOrEmpty(createDto.Definition))
//         {
//             response.AddError("definition", "Definition must not be empty");
//         }
//
//         if (response.HasErrors)
//         {
//             return BadRequest(response);
//         }
//
//         var flashcardToCreate = new Flashcard
//         {
//             Term = createDto.Term,
//             Definition = createDto.Definition,
//         };
//         _dataContext.Set<Flashcard>().Add(flashcardToCreate);
//         _dataContext.SaveChanges();
//
//         var flashcardToReturn = new FlashcardGetDto
//         {
//             Id = flashcardToCreate.Id,
//             Term = flashcardToCreate.Term,
//             Definition = flashcardToCreate.Definition,
//         };
//         response.Data = flashcardToReturn;
//         return Created("", response);
//     }
//
//     
//
//     [HttpPut("{id}")]
//     public IActionResult Update([FromBody] FlashcardUpdateDto updateDto, int id)
//     {
//         var response = new Response();
//         
//         var flashcardToUpdate = _dataContext.Set<Flashcard>().FirstOrDefault(flashcard => flashcard.Id == id);
//
//         if (string.IsNullOrEmpty(updateDto.Term))
//         {
//             response.AddError("term", "Term cannot be empty.");
//         }
//
//         if (string.IsNullOrEmpty(updateDto.Definition))
//         {
//             response.AddError("definition", "Definition cannot be empty.");
//         }
//
//         if (flashcardToUpdate == null)
//         {
//             response.AddError("id", "Flashcard Not Found.");
//         }
//         if (response.HasErrors)
//         {
//             return BadRequest(response);
//         }
//
//
//         flashcardToUpdate.Term = updateDto.Term;
//         flashcardToUpdate.Definition = updateDto.Definition;
//         
//         _dataContext.SaveChanges();
//
//         var flashcardToReturn = new FlashcardGetDto()
//         {
//             Id = flashcardToUpdate.Id,
//             Term = flashcardToUpdate.Term,
//             Definition = flashcardToUpdate.Definition,
//         };
//         
//         response.Data = flashcardToReturn;
//         return Ok(response);
//     }
//
//     [HttpDelete("{id}")]
//     public IActionResult Delete(int id)
//     {
//         var response = new Response();
//         
//         var flashcardToDelete = _dataContext.Set<Flashcard>().FirstOrDefault(flashcard => flashcard.Id == id);
//         if (flashcardToDelete == null)
//         {
//             response.AddError("id", "Flashcard Not Found.");
//         }
//
//         if (response.HasErrors)
//         {
//             return BadRequest(response);
//         }
//
//         _dataContext.Set<Flashcard>().Remove(flashcardToDelete);
//         _dataContext.SaveChanges();
//
//         response.Data = true;
//         return Ok(response);
//     }
// }