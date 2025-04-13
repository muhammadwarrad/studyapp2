using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudyApp.Data;
using StudyApp.Entities;
using StudyApp.Common;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StudySessionController : ControllerBase
{
    private readonly DataContext _context;

    public StudySessionController(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> StartStudySession([FromBody] StartStudySessionDto request)
    {
        var response = new Response();

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            response.AddError("userId", "User not found");
            return NotFound(response);
        }

        var flashcardSet = await _context.FlashcardSets.FindAsync(request.FlashcardSetId);
        if (flashcardSet == null)
        {
            response.AddError("flashcardSetId", "Flashcard set not found");
            return NotFound(response);
        }

        if (request.UserId != GetCurrentUserId())
        {
            response.AddError("userId", "Unauthorized: You can only start a study session for yourself");
            return Unauthorized(response);
        }

        var studySession = new StudySession
        {
            UserId = request.UserId,
            FlashcardSetId = request.FlashcardSetId,
            StartTime = DateTime.UtcNow,
            Status = "in_progress"
        };

        _context.StudySessions.Add(studySession);
        await _context.SaveChangesAsync();

        var studySessionDto = new StudySessionDto
        {
            Id = studySession.Id,
            UserId = studySession.UserId,
            FlashcardSetId = studySession.FlashcardSetId,
            StartTime = studySession.StartTime,
            EndTime = studySession.EndTime,
            Status = studySession.Status
        };

        response.Data = studySessionDto;
        return CreatedAtAction(nameof(GetStudySession), new { id = studySession.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudySession(int id)
    {
        var response = new Response();

        var studySession = await _context.StudySessions
            .Include(ss => ss.FlashcardSet)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (studySession == null)
        {
            response.AddError("id", "Study session not found");
            return NotFound(response);
        }

        if (studySession.UserId != GetCurrentUserId())
        {
            response.AddError("userId", "Unauthorized: You can only view your own study sessions");
            return Unauthorized(response);
        }

        var studySessionDto = new StudySessionDto
        {
            Id = studySession.Id,
            UserId = studySession.UserId,
            FlashcardSetId = studySession.FlashcardSetId,
            StartTime = studySession.StartTime,
            EndTime = studySession.EndTime,
            Status = studySession.Status
        };

        response.Data = studySessionDto;
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudySession(int id, [FromBody] UpdateStudySessionDto request)
    {
        var response = new Response();

        var studySession = await _context.StudySessions.FindAsync(id);
        if (studySession == null)
        {
            response.AddError("id", "Study session not found");
            return NotFound(response);
        }

        if (studySession.UserId != GetCurrentUserId())
        {
            response.AddError("userId", "Unauthorized: You can only update your own study sessions");
            return Unauthorized(response);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            var validStatuses = new[] { "in_progress", "completed" };
            if (!validStatuses.Contains(request.Status))
            {
                response.AddError("status", "Invalid status. Must be 'in_progress' or 'completed'");
            }
        }

        if (response.HasErrors)
        {
            return BadRequest(response);
        }

        studySession.EndTime = request.EndTime ?? studySession.EndTime;
        studySession.Status = request.Status ?? studySession.Status;

        await _context.SaveChangesAsync();

        var studySessionDto = new StudySessionDto
        {
            Id = studySession.Id,
            UserId = studySession.UserId,
            FlashcardSetId = studySession.FlashcardSetId,
            StartTime = studySession.StartTime,
            EndTime = studySession.EndTime,
            Status = studySession.Status
        };

        response.Data = studySessionDto;
        return Ok(response);
    }

    [HttpGet("/api/users/{userId}/studySession")]
    public async Task<IActionResult> GetUserStudySessions(int userId, [FromQuery] int? flashcardSetId, [FromQuery] string status)
    {
        var response = new Response();

        if (userId != GetCurrentUserId())
        {
            response.AddError("userId", "Unauthorized: You can only view your own study sessions");
            return Unauthorized(response);
        }

        var query = _context.StudySessions
            .Where(ss => ss.UserId == userId)
            .AsQueryable();

        if (flashcardSetId.HasValue)
        {
            query = query.Where(ss => ss.FlashcardSetId == flashcardSetId.Value);
        }
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(ss => ss.Status == status);
        }

        var studySessions = await query
            .Select(ss => new StudySessionDto
            {
                Id = ss.Id,
                UserId = ss.UserId,
                FlashcardSetId = ss.FlashcardSetId,
                StartTime = ss.StartTime,
                EndTime = ss.EndTime,
                Status = ss.Status
            })
            .ToListAsync();

        response.Data = studySessions;
        return Ok(response);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return int.Parse(userIdClaim);
    }
}