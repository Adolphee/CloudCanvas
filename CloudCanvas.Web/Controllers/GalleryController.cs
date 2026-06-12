using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using CloudCanvas.Web.Data;
using CloudCanvas.Web.Migrations;
using CloudCanvas.Web.Models;
using CloudCanvas.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController(ILogger<GalleryController> logger, CosmosClientWrapper cosmos_wrapper, UserManager<ApplicationUser> user_mgr) : Controller
    {
        private readonly ILogger<GalleryController> _logger = logger;
        private readonly ICosmosClientWrapper _cosmos = cosmos_wrapper;
        private readonly UserManager<ApplicationUser> _userMgr = user_mgr;

        [HttpGet, Authorize]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("[GET] Getting gallery items from {service}...", nameof(BlobStorageService));
            var blobs = await _cosmos.ListBlobsAsync<GalleryItemDTO>(CloudCosmos.Containers.BlobMeta);
            _logger.LogInformation("[GET] Succesfully obtained gallery items from {service}...", nameof(BlobStorageService));
            var user = await _userMgr.GetUserAsync(User); // Authorize enforces this object not to be nul?
            var displayName = user?.DisplayName ?? user?.FirstName ?? "Unknown User";
            return View("GalleryItemsList", new GalleryViewModel(await GetTableRowsAsync(blobs), (user?.Id ?? String.Empty),displayName));
        }

        public async Task<List<TableRowPhoto>> GetTableRowsAsync(IEnumerable<GalleryItemDTO> blobs)
        {
            var userIds = blobs.Select(r => r.UserId).ToList();
            var users = _userMgr.Users.Where(u => userIds.Contains(u.Id)).ToList();
            var authors = users.ToDictionary(u =>  u.Id, u => u);

            return blobs.Select(b =>
            {
                var a = authors.TryGetValue(b.UserId!, out var author);
                b.Thumbnails.TryGetValue(ThumbnailSize.small, out var thumbnailUrl);
                return new TableRowPhoto
                {
                    Id = b.Id,
                    Url = b.Url,
                    ThumbnailUrl = thumbnailUrl ?? "https://placehold.jp/50x50.png",
                    DisplayName = b.DisplayName ?? b.OriginalFilename,
                    Description = String.IsNullOrWhiteSpace(b.Description) ? "No Description": b.Description,
                    LastModified = b.LastModified,
                    CreatedOn = b.CreatedOn,
                    GalleryUrl = b.GalleryName ?? "#",
                    GalleryName = b.GalleryName ?? "Public Gallery",
                    AuthorDisplayName = author?.DisplayName ?? author?.FirstName ?? "Anonymous User",
                    AuthorId = b.UserId!,
                    ContentLength = b.ContentLength,
                    ContainerName = b.ContainerName
                };
            }).ToList();
        }
    }
}