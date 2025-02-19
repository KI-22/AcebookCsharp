namespace acebook.Models;
using Microsoft.EntityFrameworkCore;

public class AcebookDbContext : DbContext
{

    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Friendship>? Friendships { get; set; }
    public DbSet<Likes>? Likes { get; set; }
    public DbSet<Comment>? Comments { get; set; }

    public string? DbPath { get; }

    public string? GetDatabaseName() {
      string? DatabaseNameArg = Environment.GetEnvironmentVariable("DATABASE_NAME");

      if( DatabaseNameArg == null)
      {
        // System.Console.WriteLine(
        //   "DATABASE_NAME is null. Defaulting to test database."
        // );
        return "acebook_csharp_test";
      }
      else
      {
        System.Console.WriteLine(
          "Connecting to " + DatabaseNameArg
        );
        return DatabaseNameArg;
      }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=1234;Database=" + GetDatabaseName());
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
          .Navigation(post => post.User)
          .AutoInclude();
        
        modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
          .IsUnique();

        modelBuilder.Entity<Friendship>()
          .HasIndex(f => new { f.User1Id, f.User2Id })
          .IsUnique(); // Prevents duplicate friendships

        //  Define Friendships Relationships Properly
        modelBuilder.Entity<Friendship>()
          .HasOne(f => f.User1)
          .WithMany(u => u.SentFriendRequests) // One User can send many requests
          .HasForeignKey(f => f.User1Id)
          .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User2)
            .WithMany(u => u.ReceivedFriendRequests) // One User can receive many requests
            .HasForeignKey(f => f.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Comment>()
          .HasOne(c => c.User)
          .WithMany(u => u.Comments)
          .HasForeignKey(c => c.UserId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}
