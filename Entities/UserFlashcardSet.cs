using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StudyApp.Entities;

public class UserFlashcardSet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int FlashcardSetId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
    public List<FlashcardDto> Flashcards { get; set; }
    
}
public class UserInfoForSet {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}

public class FlashcardSetForUserDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<FlashcardDto> Flashcards { get; set; }
    //public List<FlashcardSet> FlashcardSets { get; set; }
}

public class UserFlashcardSetEntityTypeConfiguration : IEntityTypeConfiguration<UserFlashcardSet>
{
    public void Configure(EntityTypeBuilder<UserFlashcardSet> builder)
    {
        builder.ToTable("UserFlashcardSets");

        builder.HasKey(x => new { x.FlashcardSetId, x.UserId});
        
      
        builder.HasOne(x => x.FlashcardSet)
            .WithMany(x => x.Users);
        builder.HasKey(x => x.UserId);
          
      
        builder.HasOne(x => x.User)
            .WithMany(x => x.FlashcardSets);
        builder.HasKey(x => x.UserId);
        
            
    }
}