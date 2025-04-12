// using LearningStarter.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace LearningStarter.Entities;
//
// public class Flashcard
// {
//     public int Id { get; set; }
//     public string FlashcardId { get; set; }
//     public string Term { get; set; }
//     public string Definition { get; set; }
//     public FlashcardSet FlashcardSet { get; set; }
//     
// }
//
// public class FlashcardGetDto
// {
//     public int Id { get; set; }
//     public string Term { get; set; }
//     public string Definition { get; set; }
//     public FlashcardSet FlashcardSet { get; set; }
// }
//
// public class FlashcardCreateDto
// {
//     public string Term { get; set; }
//     public string Definition { get; set; }
// }
//
// public class FlashcardUpdateDto
// {
//     public string Term { get; set; }
//     public string Definition { get; set; }
// }
//
// public class FlashcardEntityTypeConfiguration : IEntityTypeConfiguration<Flashcard>
// {
//     public void Configure(EntityTypeBuilder<Flashcard> builder)
//     {
//         builder.ToTable("Flashcards");
//     }
// }