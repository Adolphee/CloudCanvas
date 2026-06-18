using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.Thumbnail;
using CloudCanvas.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Infrastructure.Persistence;

public class CCDBContext(DbContextOptions<CCDBContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers")
            .HasMany<Post>().WithOne().OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Post>(p =>
        {
            p.ToTable("Posts").UseTptMappingStrategy();
            p.HasMany<Reaction>().WithOne(r => r.Post).HasForeignKey(r => r.PostId).OnDelete(DeleteBehavior.NoAction);
            p.HasMany<Comment>().WithOne(c => c.Post).HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Reaction>(r =>
        {
            r.ToTable("Reactions").UseTptMappingStrategy();
            r.HasIndex(r => new { r.UserId, r.PostId, r.Type })
            .IsUnique();
        });

        modelBuilder.Entity<Comment>().ToTable("Comments");

        modelBuilder.Entity<PhotoThumbnail>().ToTable("PhotoThumbnails");

        modelBuilder.Entity<Photo>().ToTable("Photos");


        modelBuilder.Entity<Gallery>().ToTable("Galleries");
            
    }
}
