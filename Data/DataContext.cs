using StudyApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudyApp.Data;

public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    //public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<FlashcardSet> FlashcardSets { get; set; }
    public DbSet<UserFlashcardSet> UserFlashcardSets { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(u => u.FlashcardSets)
            .WithOne(ufs => ufs.User)
            .HasForeignKey(ufs => ufs.UserId);

        builder.Entity<FlashcardSet>()
            .HasMany(fs => fs.Flashcards)
            .WithOne(f => f.FlashcardSet)
            .HasForeignKey(f => f.FlashcardSetId);

        builder.Entity<FlashcardSet>()
            .HasMany(fs => fs.Users)
            .WithOne(ufs => ufs.FlashcardSet)
            .HasForeignKey(ufs => ufs.FlashcardSetId);

        builder.Entity<UserFlashcardSet>()
            .HasKey(ufs => new { ufs.UserId, ufs.FlashcardSetId });

        builder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        builder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(ur => ur.RoleId);

        builder.Entity<StudySession>()
            .HasOne(ss => ss.User)
            .WithMany()
            .HasForeignKey(ss => ss.UserId);

        builder.Entity<StudySession>()
            .HasOne(ss => ss.FlashcardSet)
            .WithMany()
            .HasForeignKey(ss => ss.FlashcardSetId);
    }
}