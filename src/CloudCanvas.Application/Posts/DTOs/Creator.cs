using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record Creator
    {
        public string? Id;
        public string? UserName;
        public string? DisplayName { get; set; }
        public Creator() { }
        public Creator(string id, string? username = null, string? displayName = null)
        {
            Id = id;
            UserName = username ?? "Unknown User";
            DisplayName = displayName ?? "No display name";
        }

        public string? GetId() => Id;
        public string? GetUserName() => UserName;
        private void ResetId() { Id = null; }
        private void ResetUserName() { UserName = null; }

        public void SetDisplayNameOnly(string? displayName)
        {
            DisplayName = displayName;
            ResetId();
            ResetUserName();
        }
    }
}
