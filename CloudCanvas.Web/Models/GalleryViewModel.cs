using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Web.Data;
using CloudCanvas.Web.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Web.Models
{
    public class GalleryViewModel
    {
        public List<TableRowPhoto> Rows { get; set; } = new();
        public string CurrentUserId { get; set; }
        public string CurrentUserDisplayName { get; set; }
        public GalleryViewModel(List<TableRowPhoto> rows, string currentUserId, string userDisplayName)
        {
            Rows = rows;
            CurrentUserDisplayName = userDisplayName;
            CurrentUserId = currentUserId;
        }
    }
}