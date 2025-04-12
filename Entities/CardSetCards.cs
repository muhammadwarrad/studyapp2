// //FlashCardSets to Flashcards relationship
// //One to many
//
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace LearningStarter.Entities;
//
// public class CardSetCards
// {
//     public int Id { get; set; }
//     public int FlashcardSetId { get; set; }
//     public FlashcardSet FlashcardSet { get; set; }
//     public int FlashcardId { get; set; }
//     public Flashcard Flashcard { get; set; }
//     
// }
//
// public class CardsForSetsDto
// {
//     public int Id { get; set; }
//     public string Term { get; set; }
//     public string Definition { get; set; }
//     
// }
//
// public class CardSetCardsEntityTypeConfiguration : IEntityTypeConfiguration<CardSetCards>
// {
//     public void Configure(EntityTypeBuilder<CardSetCards> builder)
//     {
//         builder.ToTable("cardSetCard");
//
//         builder.HasKey(x => new { x.FlashcardSetId, x.FlashcardId });
//         builder.HasOne(x => x.FlashcardSet)
//             .WithMany(x => x.Flashcards);
//        
//     }
// }