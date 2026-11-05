using CloudCanvas.Application.Posts.Galleries.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public sealed class GalleryRepository (CCDBContext context, ILogger<Gallery> logger) : PostRepositoryBase<Gallery>(context, logger), IGalleryRepository;
}
