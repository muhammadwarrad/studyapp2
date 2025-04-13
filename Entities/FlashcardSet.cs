using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LearningStarter.Entities;


public class FlashcardSet
{
    public int Id { get; set; }
    public string FlashcardSetId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int FlashcardId { get; set; }
    public List<FlashcardDto> Flashcards { get; set; } 
    public List<UserFlashcardSet> Users { get; set; }

}


public class FlashcardSetGetDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int FlashcardId { get; set; }
    public List<FlashcardGetDto> Flashcards { get; set; }
    public List<UserInfoForSet> Users { get; set; }
}


public class FlashcardSetCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int FlashcardId { get; set; }
    public ICollection<FlashcardGetDto> Flashcards { get; set; }
  
}


public class FlashcardSetUpdateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ICollection<FlashcardGetDto> Flashcards { get; set; }
    
}

public class FlashcardDto
{
    public int Id { get; set; }
    
    public string Front { get; set; }
    public string Back { get; set; }
    public int FlashcardSetId { get; set; }
    public FlashcardSet FlashcardSet { get; set; }
}

public class FlashcardGetDto
{
    public int Id { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public int FlashcardSetId { get; set; }
}

public class FlashcardCreateDto
{
    public string Front { get; set; }
    public string Back { get; set; }
    //public int FlashcardSetId { get; set; }
}


public class FlashcardSetEntityTypeConfiguration : IEntityTypeConfiguration<FlashcardSet>
{
    public void Configure(EntityTypeBuilder<FlashcardSet> builder)
    {
        builder.ToTable("FlashcardSets");
        builder.Property(x => x.FlashcardSetId);
        builder.Property(x => x.FlashcardId);
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Flashcards);
    }
}