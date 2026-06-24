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
        modelBuilder.Entity<ApplicationUser>(u => {
            u.HasMany<Gallery>().WithOne().HasConstraintName("SingleUser_ManyGalleries");
        });

        modelBuilder.Entity<Post>(p =>
        {
            p.ToTable("Posts").UseTptMappingStrategy();
            p.HasOne<ApplicationUser>().WithMany(p => p.Posts)
            .HasConstraintName("SingleUser_ManyPosts")
            .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Reaction>(r =>
        {
            r.ToTable("Reactions").UseTptMappingStrategy();
            r.HasIndex(r => new { r.UserId, r.PostId, r.Type }).IsUnique();
        });


        modelBuilder.Entity<Photo>(p =>
        {
            p.ToTable("Photos");
            p.HasOne<Gallery>().WithMany(g => g.Photos).HasForeignKey(p => p.GalleryId)
            .HasConstraintName("SingleGallery_ManyPhotos")
            .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Comment>().ToTable("Comments");
        modelBuilder.Entity<PhotoThumbnail>().ToTable("PhotoThumbnails");
        modelBuilder.Entity<Gallery>().ToTable("Galleries");
    }
}
