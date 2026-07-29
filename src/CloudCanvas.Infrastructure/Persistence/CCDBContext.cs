using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Reactions.Entities;
using CloudCanvas.Domain.Thumbnail;
using CloudCanvas.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Infrastructure.Persistence;

public class CCDBContext(DbContextOptions<CCDBContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<PhotoThumbnail> Thumbnails { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(u =>
        {
            u.HasMany(u => u.Posts).WithOne().HasForeignKey(p => p.UserId).HasConstraintName("SingleUser_ManyPosts").OnDelete(DeleteBehavior.NoAction);
            u.HasMany(u => u.Reactions).WithOne().HasForeignKey(r => r.UserId).HasConstraintName("SingleUser_ManyReactions").OnDelete(DeleteBehavior.NoAction);
            u.HasMany(u => u.Comments).WithOne().HasForeignKey(c => c.UserId).HasConstraintName("SingleUser_ManyComments").OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Post>(p =>
        {
            p.ToTable("Posts").UseTptMappingStrategy();
            p.HasMany(p => p.Comments).WithOne(c => c.Post).HasForeignKey(c => c.PostId).HasConstraintName("SinglePost_ManyComments").OnDelete(DeleteBehavior.NoAction);
            p.HasMany(p => p.Reactions).WithOne(r => r.Post).HasForeignKey(r => r.PostId).HasConstraintName("SinglePost_ManyReactions").OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Reaction>(r =>
        {
            r.ToTable("Reactions").UseTptMappingStrategy();
            r.HasIndex(r => new { r.UserId, r.PostId, r.Type }).IsUnique();
        });

        modelBuilder.Entity<Photo>(p =>
        {
            p.ToTable("Photos");
            p.HasMany(p => p.Thumbnails).WithOne(t => t.OriginalPhoto).HasForeignKey(p => p.PhotoId).HasConstraintName("SinglePhoto_ManyThumbnails").OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Comment>().ToTable("Comments");
        modelBuilder.Entity<PhotoThumbnail>().ToTable("PhotoThumbnails").HasIndex(t => new { t.Size, t.PhotoId }, "SinglePhotoManyThumbnails_OnlyDifferentSizes").IsUnique();
        modelBuilder.Entity<Gallery>(g =>
        {
            g.ToTable("Galleries");
            g.HasMany(p => p.Photos).WithOne(g => g.Gallery).HasForeignKey(p => p.GalleryId).HasConstraintName("SingleGallery_ManyPhotos").OnDelete(DeleteBehavior.NoAction);
        });
    }
}
