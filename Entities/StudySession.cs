using System;

namespace StudyApp.Entities;

public class StudySession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FlashcardSetId { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
    
    // Navigation properties
    public User User { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
}

public class StudySessionDto 
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FlashcardSetId { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
}

public class StartStudySessionDto
{
    public int UserId { get; set; }
    public int FlashcardSetId { get; set; } 
}

public class UpdateStudySessionDto
{
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
}