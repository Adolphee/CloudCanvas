using CloudCanvas.Web.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Web.Data;

public class CCDBContext(DbContextOptions<CCDBContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<CloudCanvas.Web.Data.Gallery> Gallery { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers")
            .HasMany(u => u.Comments)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Post>(p =>
        {
            p.ToTable("Posts").UseTptMappingStrategy();
            p.HasOne(p => p.Author).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
            p.HasMany(p => p.Dislikes).WithOne().OnDelete(DeleteBehavior.NoAction);
            p.HasMany(p => p.Comments).WithOne().OnDelete(DeleteBehavior.NoAction);
            p.HasMany(p => p.Likes).WithOne().OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Reaction>(r =>
        {
            r.ToTable("Reactions").UseTptMappingStrategy();r.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.NoAction);
            r.HasIndex(r => new { r.UserId, r.PostId, r.Type })
            .IsUnique();
        });

        modelBuilder.Entity<Comment>().ToTable("Comments")
            .HasOne(c => c.TargetPost)
            .WithMany()
            .HasForeignKey(c => c.PostId).HasConstraintName("CommentToPostId")
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<PhotoThumbnail>().ToTable("PhotoThumbnails")
            .HasOne(c => c.OriginalPhoto)
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Photo>().ToTable("Photos")
            .HasMany(c => c.Thumbnails)
            .WithOne()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);
            

        modelBuilder.Entity<Gallery>().ToTable("Galleries")
            .HasMany(g => g.Photos)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>().ToTable("Likes");
        modelBuilder.Entity<Dislike>().ToTable("Dislikes");
        modelBuilder.Entity<EmojiReaction>().ToTable("EmojiReactions");
    }
}
